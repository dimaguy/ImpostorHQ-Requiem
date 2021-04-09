using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Http.Server.Request.Fields;

namespace ImpostorHqR.Core.Web.Http.Server.Request
{
    public static class HttpRequestParser
    {
        private static readonly char[] RangeField = new char[] { 'R', 'a', 'n', 'g', 'e', ':' };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HttpInitialRequest? ParseRequest(string str)
        {
            var data = str.AsSpan();

            var result = new HttpInitialRequest();

            if (str.StartsWith("GET ")) result.Method = HttpInitialRequestMethod.GET;
            else if (str.StartsWith("HEAD ")) result.Method = HttpInitialRequestMethod.HEAD;
            else return null;

            var start = result.Method == HttpInitialRequestMethod.HEAD ? 5 : 4;
            data = data.Slice(start);

            var path = data.Slice(1, data.IndexOf(' ') - 1);
            result.Path = new string(path);

            var version = data.Slice(path.Length + 2, 8);
            result.HttpVersion = new string(version);

            var remaining = data.Slice(path.Length + version.Length + 2);

            var rangeIndex = remaining.IndexOf(RangeField);

            if (rangeIndex == -1) return result;
            var range = remaining.Slice(rangeIndex + 6);
            var newLine = range.IndexOf('\n');
            var content = (newLine != -1 ? new string(range.Slice(0, newLine)) : new string(range)).Trim();

            if (!RangeHeaderValue.TryParse(content, out var value)) return result;
            if (value.Ranges.Count == 0) return result;
            var list = new List<HttpRequestRangeField>();
            foreach (var rangeValue in value.Ranges)
            {
                var val = new HttpRequestRangeField();

                if (rangeValue.From == null && rangeValue.To != null)
                {
                    val.Method = HttpRangeRequestMethod.SliceFromStartTo;
                    val.Range = rangeValue;
                    list.Add(val);
                }
                else if (rangeValue.From != null && rangeValue.From != 0 && rangeValue.To != null && rangeValue.To > rangeValue.From)
                {
                    val.Method = HttpRangeRequestMethod.SliceFromTo;
                    val.Range = rangeValue;
                    list.Add(val);
                }
                else if (rangeValue.From != null && rangeValue.From == 0 && rangeValue.To == null)
                {
                    val.Method = HttpRangeRequestMethod.SendAll;
                    val.Range = rangeValue;
                    list.Add(val);
                }
                else if (rangeValue.From != null && rangeValue.From != 0 && rangeValue.To == null)
                {
                    val.Method = HttpRangeRequestMethod.SliceFromToEnd;
                    val.Range = rangeValue;
                    list.Add(val);
                }
                else
                {
                    ConsoleLogging.Instance.LogError($"Unknown HTTP range method: {rangeValue.From} - {rangeValue.To}", null, true);
                }
            }
            result.Ranges = list;

            return result;
        }

    }
}
