using System.Net.Http.Headers;

namespace ImpostorHqR.Core.Web.Http.Server.Request.Fields
{
    public struct HttpRequestRangeField
    {
        public RangeItemHeaderValue Range { get; set; }

        public HttpRangeRequestMethod Method { get; set; }
    }

    public enum HttpRangeRequestMethod
    {
        SendAll,            // 0 - 
        SliceFromToEnd,     // x - 
        SliceFromTo,        // x - y
        SliceFromStartTo    //   - y
    }
}
