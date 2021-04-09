using System.Drawing;
using System.Text.Json;
using ImpostorHqR.Core.Web.Api.WebSockets;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage
{
    public class SimpleApiPageSetMessage : IHqApiOutgoingMessage
    {
        public bool Clear => false;

        public bool Set => true;

        public string Message { get; }

        public string Color { get; }

        public SimpleApiPageSetMessage(string text, Color? color)
        {
            if (color != null) this.Color = $"rgb({color?.R}, {color?.G}, {color?.B})";
            this.Message = text;
        }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
