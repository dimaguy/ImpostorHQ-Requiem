using ImpostorHqR.Extensions.Api.Interface.Game;

namespace ImpostorHqR.Core.Impostor.Chat
{
    public class ChatHolder : IChatUtils
    {
        public static readonly IChatUtils Instance = new ChatHolder();

        public IChatWriter MessageWriter => ChatWriter.Instance;

        public IChatMessageProvider MessageProvider => ChatMessageProvider.Instance;
    }
}
