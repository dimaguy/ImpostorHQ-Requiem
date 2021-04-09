using System.Drawing;
using System.Text.Json;
using ImpostorHqR.Core.Web.Api.WebSockets;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Simple;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage
{
    public class SimpleApiPageElement : IHqApiOutgoingMessage, ISimpleApiPageElement
    {
        public string Message { get; }

        public string Color { get; }

        public Color BackgroundColor { get; }

        public SimpleApiPageElement(string message, Color? color)
        {
            this.Message = message;
            if (Color != null)
            {
                this.BackgroundColor = (Color)color;
                this.Color = $"rgb({color?.R}, {color?.G}, {color?.B})";
            }
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
