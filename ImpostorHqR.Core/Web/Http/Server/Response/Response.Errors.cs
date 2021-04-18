using System.IO;
using System.Text;
using ImpostorHqR.Core.Web.Http.Server.Response.Fields;

namespace ImpostorHqR.Core.Web.Http.Server.Response
{
    public static class HttpErrorResponses
    {
        public static byte[] NotFoundResponse;

        public static byte[] NotImplementedResponse;

        public static byte[] NotAuthorizedResponse;

        public static void Initialize()
        {
            #region Not Found 404

            using var notFoundDocument = new MemoryStream();
            using var notFoundHeader = new HttpResponseHeaders(HttpConstant.NotFound404Message.Length, ResponseStatusCode.FileNotFound404, new IResponseField[]
            {
                new FieldServer(HttpConstant.ServerName),
                new FieldAcceptRanges(HttpConstant.AcceptRanges),
                new FieldContentType("text/html")
            }, "HTTP/1.1");
            var nfh = notFoundHeader.Compile();
            notFoundDocument.Write(nfh.Item1,0,nfh.Item2);
            notFoundDocument.Write(Encoding.UTF8.GetBytes(HttpConstant.NotFound404Message));
            NotFoundResponse = notFoundDocument.ToArray();

            #endregion

            #region Not Implemented 501
            
            using var notImplementedDocument = new MemoryStream();
            using var notImplementedHeader = new HttpResponseHeaders(HttpConstant.NotImplemented501.Length, ResponseStatusCode.NotImplemented501, new IResponseField[]
            {
                new FieldServer(HttpConstant.ServerName),
                new FieldAcceptRanges(HttpConstant.AcceptRanges),
                new FieldContentType("text/html")
            }, "HTTP/1.1");
            var nid = notImplementedHeader.Compile();
            notImplementedDocument.Write(nid.Item1,0,nid.Item2);
            notImplementedDocument.Write(Encoding.UTF8.GetBytes(HttpConstant.NotImplemented501));
            NotImplementedResponse = notImplementedDocument.ToArray();

            #endregion

            #region Not Authorized 401

            using var notAuthorizedDocument = new MemoryStream();
            using var notAuthorizedHeader = new HttpResponseHeaders(HttpConstant.NotAuthorized401.Length, ResponseStatusCode.Unauthorized401, new IResponseField[]
            {
                new FieldServer(HttpConstant.ServerName),
                new FieldAcceptRanges(HttpConstant.AcceptRanges),
                new FieldContentType("text/html"),
                new FieldAuthentication("Basic", "Gallifrey"),
            }, "HTTP/1.1");
            var nah = notAuthorizedHeader.Compile();
            notAuthorizedDocument.Write(nah.Item1, 0, nah.Item2);
            notAuthorizedDocument.Write(Encoding.UTF8.GetBytes(HttpConstant.NotAuthorized401));
            NotAuthorizedResponse = notAuthorizedDocument.ToArray();

            #endregion
        }
    }
}
