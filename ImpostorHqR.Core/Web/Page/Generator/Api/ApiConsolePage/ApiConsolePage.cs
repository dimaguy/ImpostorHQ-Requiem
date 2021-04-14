using System.Drawing;
using System.IO;
using System.Text;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Api.WebSockets.Handles;
using ImpostorHqR.Core.Web.Http.Handler;
using ImpostorHqR.Core.Web.Http.Server;
using ImpostorHqR.Core.Web.Http.Server.Response;
using ImpostorHqR.Core.Web.Http.Server.Response.Fields;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiConsolePage.Splicer;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage;
using ImpostorHqR.Extension.Api.Interface.Logging;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Console;
using ImpostorHqR.Extensions.Api.Interface.Logging;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiConsolePage
{
    public class ApiConsolePage : IReadonlyConsolePage
    {
        public string Handle { get; }

        public string Title { get; }

        public string Placeholder { get; }

        public Color BoxColor { get; }

        public ApiHandleHolder Handler { get; }

        public string Html { get; }

        private byte[] HtmlBytes { get; }

        public ApiConsolePage(Color boxColor, string title, string handle, string placeholder)
        {
            this.Handle = handle;
            this.Title = title;
            this.Placeholder = placeholder;
            this.BoxColor = boxColor;

            this.Html = ApiConsolePageSplicer.Html.Replace(ApiConsolePageSplicer.ReplaceInTitle, title);
            this.Html = this.Html.Replace(ApiConsolePageSplicer.ReplaceInColor, GetColor());
            this.Html = this.Html.Replace(ApiConsolePageSplicer.ReplaceInPort, ConfigHolder.Instance.ApiPort.ToString());
            this.Html = this.Html.Replace(ApiConsolePageSplicer.ReplaceInHandle, handle);
            this.Html = this.Html.Replace(ApiConsolePageSplicer.ReplaceInMainPlaceholder, placeholder);

            this.Handler = new ApiHandleHolder(handle);
            WebApiHandleStore.Instance.Add(this.Handler);

            using (var ms = new MemoryStream())
            {
                var headers = new HttpResponseHeaders(this.Html.Length, ResponseStatusCode.Ok200, new IResponseField[]
                {
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
                    Message = $"Served Simple Console Api Page [{handle}] to {client.Client.RemoteEndPoint}.",
                    Source = this,
                    Type = LogType.Debug
                });
            });
            HttpHandleStore.Instance.AddHandler(webHandle);
            ConsoleLogging.Instance.LogInformation($"Registered simple console api page at [{handle}].", true);
        }

        private string GetColor()
        {
            return $"rgb({BoxColor.R}, {BoxColor.G}, {BoxColor.B})";
        }

        public void Push(string text)
        {
            Handler.Push(new SimpleApiPageElement(text, null));
        }

        public void Clear()
        {
            Handler.Push(new SimpleApiPageClearMessage());
        }

        public void Set(string text)
        {
            Handler.Push(new SimpleApiPageSetMessage(text, null));
        }
    }
}
