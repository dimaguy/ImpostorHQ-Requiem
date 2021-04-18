#nullable enable
using System.Collections.Generic;
using ImpostorHqR.Core.Web.Http.Server.Request.Fields;

namespace ImpostorHqR.Core.Web.Http.Server.Request
{

    public enum HttpInitialRequestMethod
    {
        GET, HEAD
    }

    public struct HttpInitialRequest
    {
        public List<HttpRequestRangeField>? Ranges { get; set; }

        public HttpInitialRequestMethod Method { get; set; }

        public string HttpVersion { get; set; }

        public string Path { get; set; }
    }
}
