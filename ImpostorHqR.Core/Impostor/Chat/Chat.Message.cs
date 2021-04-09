using System.Drawing;
using ImpostorHqR.Extensions.Api.Interface.Game;

namespace ImpostorHqR.Core.Impostor.Chat
{
    public struct ChatMessage : IChatMessage
    {
        public string[] Messages { get; set; }

        public string SourceName { get; set; }

        public Color Color { get; set; }

        public string GenerateColor()
        {
            return $"{Color.R:X2}{Color.G:X2}{Color.B:X2}{Color.A:X2}";
        }
    }
}
