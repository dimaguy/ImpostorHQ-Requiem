using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Http.Handler;
using ImpostorHqR.Core.Web.Http.Server.Client;
using ImpostorHqR.Core.Web.Http.Server.Request.Fields;
using ImpostorHqR.Core.Web.Http.Server.Response;

namespace ImpostorHqR.Core.Web.Http.Server.Request
{
    public static class HttpRequestParser
    {
        private static readonly char[] RangeField = "Range:".ToCharArray();
        private static readonly char[] AuthorizationField = "Authorization:".ToCharArray();

        private static readonly bool Windows = (System.Environment.OSVersion.Platform == PlatformID.Win32NT);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static HttpInitialRequest? ParseRequest(string str, Stream transport)
        {
            var data = str.AsSpan();

            var result = new HttpInitialRequest();

            if (str.StartsWith("GET ")) result.Method = HttpInitialRequestMethod.GET;
            else if (str.StartsWith("HEAD ")) result.Method = HttpInitialRequestMethod.HEAD;
            else return null;

            var start = result.Method == HttpInitialRequestMethod.HEAD ? 5 : 4;
            data = data.Slice(start);

            var path = data.Slice(1, data.IndexOf(' ') - 1);
            if (path.Length == 0 || isEmpty(path))
            {
                transport.WriteAsync(HttpErrorResponses.NotFoundResponse).ConfigureAwait(false);
                return null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool isEmpty(ReadOnlySpan<char> chars)
            {
                foreach (var c in chars)
                {
                    if (!char.IsWhiteSpace(c)) return false;
                }

                return true;
            }

            VerifyPathPlatform(ref path);
            result.Path = new string(path);

            var version = data.Slice(path.Length + 2, 8);
            result.HttpVersion = new string(version);

            var remaining = data.Slice(path.Length + version.Length + 2);

            var rangeIndex = remaining.IndexOf(RangeField);

            if (rangeIndex != -1)
            {
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
                    else if (rangeValue.From != null && rangeValue.From != 0 && rangeValue.To != null &&
                             rangeValue.To > rangeValue.From)
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
                }

                result.Ranges = list;
            }

            var authorizationIndex = remaining.IndexOf(AuthorizationField);
            if (authorizationIndex == -1) return result;

            var authorization = remaining.Slice(authorizationIndex + 14);
            var nl = authorization.IndexOf('\n');
            var auth = (nl != -1 ? new string(authorization.Slice(0, nl)) : new string(authorization)).Trim();

            var spaceIndex = auth.IndexOf(' ');
            if (spaceIndex == -1 || auth.Length - spaceIndex < 2) return null; // malformed request
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(new string(auth.AsSpan().Slice(spaceIndex))));

            var tokens = credentials.Split(':', 2);

            if (tokens.Length != 2) return null; //malformed data

            result.Credentials = new SpecialHandler.HttpAuthOptions(tokens[0], tokens[1]);

            return result;
        }

        // !!! Unsafe! Breaks trust! Don't do this anywhere else!
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void VerifyPathPlatform(ref ReadOnlySpan<char> str)
        {
            if (Windows)
            {
                fixed (char* ptr = &str[0])
                {
                    for (var i = 0; i < str.Length; i++)
                    {
                        if (ptr[i] == '/') ptr[i] = '\\';
                    }
                }
            }
        }
    }
}
