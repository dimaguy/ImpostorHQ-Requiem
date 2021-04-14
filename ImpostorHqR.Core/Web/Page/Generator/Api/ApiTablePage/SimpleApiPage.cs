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
using ImpostorHqR.Extension.Api.Interface.Logging;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Simple;
using ImpostorHqR.Extensions.Api.Interface.Logging;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage
{
    public class SimpleApiPage : ISimpleApiPage
    {
        public string Handle { get; }

        public string Title { get; }

        public Color ElementColor { get; }

        public ApiHandleHolder Handler { get; }

        public string Html { get; }

        private byte[] HtmlBytes { get; }

        public SimpleApiPage(Color elementColor, string title, string handle)
        {
            if (WebApiHandleStore.Instance.Handles.Any(h => h.HandleId.Equals(handle)))
            {
                ConsoleLogging.Instance.LogError($"API Handler with the same name [{handle}] already exists. The page will not be registered.", this, true);
                return;
            }

            if (HttpHandleStore.Instance.Handles.Any(h => h.Path.Equals(handle)))
            {
                ConsoleLogging.Instance.LogError($"Http handle with the same name [{handle}] already exists. The page will not be registered.", this, true);
                return;
            }


            this.Title = title;
            this.Handle = handle;
            this.ElementColor = elementColor;
            this.Handler = new ApiHandleHolder(handle);
            WebApiHandleStore.Instance.Add(this.Handler);
            // compile page
            this.Html = SimpleApiPageSplicerConstant.Page.Replace(SimpleApiPageSplicerConstant.ReplaceInTitle, title);
            this.Html = this.Html.Replace(SimpleApiPageSplicerConstant.ReplaceInColor, GetColor());
            this.Html = this.Html.Replace(SimpleApiPageSplicerConstant.ReplaceInPort, ConfigHolder.Instance.ApiPort.ToString());
            this.Html = this.Html.Replace(SimpleApiPageSplicerConstant.ReplaceInHandle, handle);
            using (var ms = new MemoryStream())
            {
                var headers = new HttpResponseHeaders(this.Html.Length, ResponseStatusCode.Ok200, new IResponseField[] {
                    new FieldAcceptRanges(HttpConstant.AcceptRanges),
                    new FieldServer(HttpConstant.ServerName),
                    new FieldContentType("text/html")
                }, "HTTP/1.1");

                ms.Write(headers.Compile());
                ms.Write(Encoding.UTF8.GetBytes(this.Html));
                this.HtmlBytes = ms.ToArray();
            }
            var webHandle = new SpecialHandler(handle, async (client) =>
            {
                await client.SafeWriteAsync(this.HtmlBytes);
                await LogManager.Instance.Log(new LogEntry()
                {
                    Message = $"Served SimpleApiPage [{handle}] to {client.Client.RemoteEndPoint}.",
                    Source = this,
                    Type = LogType.Debug
                });
            });
            HttpHandleStore.Instance.AddHandler(webHandle);
            ConsoleLogging.Instance.LogInformation($"Registered simple api page at [{handle}].", true);
        }

        private string GetColor()
        {
            return $"rgb({ElementColor.R}, {ElementColor.G}, {ElementColor.B})";
        }

        public void Push(ISimpleApiPageElement element)
        {
            Handler.Push(element as SimpleApiPageElement);
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
