using System.Drawing;
using ImpostorHqR.Extensions.Api.Interface.Game;

namespace ImpostorHqR.Core.Impostor.Chat
{
    public class ChatMessageProvider : IChatMessageProvider
    {
        public static readonly IChatMessageProvider Instance = new ChatMessageProvider();

        public IChatMessage Create(string sourceName, string[] messages, Color color)
        {
            return new ChatMessage()
            {
                Color = color,
                Messages = messages,
                SourceName = sourceName
            };
        }
    }
}
