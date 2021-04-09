using System.IO;
using System.Text;
using ImpostorHqR.Core.Web.Http.Server.Response.Fields;

namespace ImpostorHqR.Core.Web.Http.Server.Response
{
    public class HttpErrorResponses
    {
        public static readonly HttpErrorResponses Instance = new HttpErrorResponses();

        public readonly byte[] NotFoundResponse;

        public readonly byte[] NotImplementedResponse;

        public HttpErrorResponses()
        {
            using var notFoundDocument = new MemoryStream();
            var notFoundHeader = new HttpResponseHeaders(HttpConstant.NotFound404Message.Length, ResponseStatusCode.FileNotFound404, new IResponseField[]
            {
                new FieldServer(HttpConstant.ServerName),
                new FieldAcceptRanges(HttpConstant.AcceptRanges),
                new FieldContentType("text/html")
            }, "HTTP/1.1");

            notFoundDocument.Write(notFoundHeader.Compile());
            notFoundDocument.Write(Encoding.UTF8.GetBytes(HttpConstant.NotFound404Message));
            this.NotFoundResponse = notFoundDocument.ToArray();

            using var notImplementedDocument = new MemoryStream();
            var notImplementedHeader = new HttpResponseHeaders(HttpConstant.NotImplemented501.Length, ResponseStatusCode.NotImplemented501, new IResponseField[]
            {
                new FieldServer(HttpConstant.ServerName),
                new FieldAcceptRanges(HttpConstant.AcceptRanges),
                new FieldContentType("text/html")
            }, "HTTP/1.1");

            notImplementedDocument.Write(notImplementedHeader.Compile());
            notImplementedDocument.Write(Encoding.UTF8.GetBytes(HttpConstant.NotImplemented501));
            this.NotImplementedResponse = notImplementedDocument.ToArray();
        }
    }
}
