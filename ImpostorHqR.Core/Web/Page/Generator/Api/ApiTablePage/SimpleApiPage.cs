using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Api.WebSockets.Handles;
using ImpostorHqR.Core.Web.Http.Handler;
using ImpostorHqR.Core.Web.Http.Server;
using ImpostorHqR.Core.Web.Http.Server.Response;
using ImpostorHqR.Core.Web.Http.Server.Response.Fields;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage.Splicer;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Api.Web;
using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage
{
    public class SimpleApiPage : IApiPage
    {
        public string Handle { get; }

        public string Title { get; }

        public Color ElementColor { get; }

        public ApiHandleHolder Handler { get; }

        public string Html { get; }

        private byte[] HtmlBytes { get; }

        public SimpleApiPage(Color elementColor, string title, string handle)
        {
            if (WebApiHandleStore.Handles.Any(h => h.HandleId.Equals(handle)))
            {
                ILogManager.Log($"API Handler with the same name [{handle}] already exists. The page will not be registered.", this.ToString(), LogType.Error);
                return;
            }

            if (HttpHandleStore.Handles.Any(h => h.Value.Path.Equals(handle)))
            {
                ILogManager.Log($"Http handle with the same name [{handle}] already exists. The page will not be registered.", this.ToString(), LogType.Error);
                return;
            }


            this.Title = title;
            this.Handle = handle;
            this.ElementColor = elementColor;
            this.Handler = new ApiHandleHolder(handle);
            WebApiHandleStore.Add(this.Handler);
            // compile page
            this.Html = SimpleApiPageSplicerConstant.Page.Replace(SimpleApiPageSplicerConstant.ReplaceInTitle, title);
            this.Html = this.Html.Replace(SimpleApiPageSplicerConstant.ReplaceInColor, GetColor());
            this.Html = this.Html.Replace(SimpleApiPageSplicerConstant.ReplaceInPort, IConfigurationStore.GetByType<RequiemConfig>().ApiPort.ToString());
            this.Html = this.Html.Replace(SimpleApiPageSplicerConstant.ReplaceInHandle, handle);
            using (var ms = new MemoryStream())
            {
                using var headers = new HttpResponseHeaders(this.Html.Length, ResponseStatusCode.Ok200, new IResponseField[] {
                    new FieldAcceptRanges(HttpConstant.AcceptRanges),
                    new FieldServer(HttpConstant.ServerName),
                    new FieldContentType("text/html")
                }, "HTTP/1.1");
                var h = headers.Compile();
                ms.Write(h.Item1,0,h.Item2);
                ms.Write(Encoding.UTF8.GetBytes(this.Html));
                this.HtmlBytes = ms.ToArray();
            }
            var webHandle = new SpecialHandler(handle, async (client) =>
            {
                await client.SafeWriteAsync(this.HtmlBytes);
            });
            HttpHandleStore.AddHandler(webHandle);
        }

        private string GetColor()
        {
            return $"rgb({ElementColor.R}, {ElementColor.G}, {ElementColor.B})";
        }

        public void Push(ApiPageElement element)
        {
            Handler.Push(new SimpleApiPageElement(element.Message, element.BackgroundColor));
        }

        public void Clear()
        {
            Handler.Push(new SimpleApiPageClearMessage());
        }

        public void Set(string text, Color color)
        {
            Handler.Push(new SimpleApiPageSetMessage(text, color));
        }
    }
}
