using System.Drawing;
using System.Text.Json;
using ImpostorHqR.Core.Web.Api.WebSockets;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage
{
    public readonly struct SimpleApiPageSetMessage : IHqApiOutgoingMessage
    {
        private static readonly string Alpha = $"rgb({System.Drawing.Color.Transparent.R}, {System.Drawing.Color.Transparent.G}, {System.Drawing.Color.Transparent.B})";

        public bool Clear { get; }

        public bool Set { get; }

        public string Message { get; }

        public string Color { get; }

        public SimpleApiPageSetMessage(string text, Color? color = null)
        {
            this.Clear = false;
            this.Set = true;
            this.Color = color != null ? $"rgba({color?.R}, {color?.G}, {color?.B}, {color?.A})" : Alpha;
            this.Message = text;
        }

        public string Serialize() => JsonSerializer.Serialize(this);
    }
}
