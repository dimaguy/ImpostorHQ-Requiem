using System.Drawing;
using System.Text.Json;
using ImpostorHqR.Core.Web.Api.WebSockets;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage
{
    public readonly struct SimpleApiPageElement : IHqApiOutgoingMessage
    {
        public string Message { get; }

        public string Color { get; }

        public Color BackgroundColor { get; }

        public SimpleApiPageElement(string message, Color? color)
        {
            this.Message = message;
            this.BackgroundColor = color ?? System.Drawing.Color.Transparent;
            this.Color = $"rgb({BackgroundColor.R}, {BackgroundColor.G}, {BackgroundColor.B})";
        }

        public string Serialize() => JsonSerializer.Serialize(this);
    }
}
