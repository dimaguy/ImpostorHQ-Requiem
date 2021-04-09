using System;
using System.Runtime.CompilerServices;
using System.Text;
using ImpostorHqR.Core.ObjectPool.Pools.StringBuilder;
using ImpostorHqR.Core.Web.Http.Server.Response.Fields;

namespace ImpostorHqR.Core.Web.Http.Server.Response
{
    public readonly struct HttpResponseHeaders : IDisposable
    {
        public int ContentLength { get; }

        public IResponseField[] Fields { get; }

        public string HttpVersion { get; }

        public ResponseStatusCode Status { get; }

        public HttpResponseHeaders(int contentLength, ResponseStatusCode code, IResponseField[] fields, string version)
        {
            this.ContentLength = contentLength;
            this.Status = code;
            this.HttpVersion = version;
            this.Fields = fields;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Compile()
        {
            using var sb = StringBuilderPool.Instance.Get();

            sb.Append(HttpVersion);
            sb.Append(" ");
            sb.Append(GetStatus());
            sb.Append("\r\n");
            var contentLenField = new FieldContentLength(this.ContentLength);

            foreach (var responseField in Fields)
            {
                sb.Append(responseField.Compile());
                sb.Append("\r\n");
            }

            sb.Append(contentLenField.Compile());
            sb.Append("\r\n\r\n");

            var result = Encoding.ASCII.GetBytes(sb.ToString() ?? throw new InvalidOperationException("What happened here? [line 47 of Response.Headers]"));
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string GetStatus()
        {
            return Status switch
            {
                ResponseStatusCode.FileNotFound404 => "404 Not Found",
                ResponseStatusCode.NotImplemented501 => "501 Not Implemented",
                ResponseStatusCode.Ok200 => "200 OK",
                ResponseStatusCode.PartialContent206 => "206 Partial Content",
                ResponseStatusCode.ServiceUnavailable503 => "503 Service Unavailable",
                _ => throw new Exception("spooky")
            };
        }

        public void Dispose()
        {
        }
    }
}
