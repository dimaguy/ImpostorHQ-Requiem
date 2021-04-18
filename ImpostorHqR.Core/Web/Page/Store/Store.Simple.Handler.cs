using System.Text;
using ImpostorHqR.Core.Web.Http.Handler;
using ImpostorHqR.Core.Web.Http.Server;
using ImpostorHqR.Core.Web.Http.Server.Response;
using ImpostorHqR.Core.Web.Http.Server.Response.Fields;
using ImpostorHqR.Core.Web.Page.Generator.NoApi.TableSite;

namespace ImpostorHqR.Core.Web.Page.Store
{
    public class SimplePageHandler
    {
        public TableSite Site { get; set; }

        public SpecialHandler Handle { get; }

        public SimplePageHandler(string name, TableSite site)
        {
            this.Site = site;
            this.Handle = new SpecialHandler(name, async (holder) =>
            {
                var document = Encoding.UTF8.GetBytes(Site.GetLatest());
                using var headers = new HttpResponseHeaders(document.Length, ResponseStatusCode.Ok200, new IResponseField[]
                {
                    new FieldServer(HttpConstant.ServerName),
                    new FieldAcceptRanges(HttpConstant.AcceptRanges),
                    new FieldContentType("text/html")
                }, "HTTP/1.1");
                var h = headers.Compile();
                if(!await holder.SafeWriteAsync(h.Item1, h.Item2)) return;
                await holder.SafeWriteAsync(document);
            });
            HttpHandleStore.AddHandler(this.Handle);
        }
    }
}
