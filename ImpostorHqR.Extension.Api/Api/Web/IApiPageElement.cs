using System.Drawing;

namespace ImpostorHqR.Extension.Api.Api.Web
{
    public readonly struct ApiPageElement
    {
        public Color BackgroundColor { get; }

        public string Message { get; }

        public ApiPageElement(Color bg, string message)
        {
            this.BackgroundColor = bg;
            this.Message = message;
        }
    }
}
