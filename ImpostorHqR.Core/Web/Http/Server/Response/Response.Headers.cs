using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using ImpostorHqR.Core.Web.Http.Server.Response.Fields;
using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Core.Web.Http.Server.Response
{
    public struct HttpResponseHeaders : IDisposable
    {
        public static readonly ArrayPool<byte> BytePoolShared = ArrayPool<byte>.Shared;

        public long ContentLength { get; }

        public IResponseField[] Fields { get; }

        public string HttpVersion { get; }

        public ResponseStatusCode Status { get; }

        public byte[] Buffer { get; private set; }

        public HttpResponseHeaders(long contentLength, ResponseStatusCode code, IResponseField[] fields, string version)
        {
            this.ContentLength = contentLength;
            this.Status = code;
            this.HttpVersion = version;
            this.Fields = fields;
            this.Buffer = null;
        }
        /// <summary>
        /// WARNING! RETURN BYTES TO THE POOL! (calling dispose)
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTuple<byte[], int> Compile()
        {
            using var sb = IReusableStringBuilder.Get();

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
            sb.Append("Connection: close");
            sb.Append("\r\n");

            sb.Append(contentLenField.Compile());
            sb.Append("\r\n\r\n");

            var str = sb.ToString();
            this.Buffer = BytePoolShared.Rent(str!.Length);

            Encoding.UTF8.GetBytes(str, Buffer.AsSpan(0, str.Length));

            return (Buffer, str.Length);
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
                ResponseStatusCode.Unauthorized401 => "401 Unauthorized",
                _ => throw new Exception("spooky")
            };
        }

        public void Dispose()
        {
            Trace.Assert(Buffer != null, "Http response was disposed before compilation.");
            BytePoolShared.Return(Buffer);
        }
    }
}
