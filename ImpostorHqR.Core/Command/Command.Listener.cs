using Impostor.Api.Events.Player;
using ImpostorHqR.Core.Command.Processor;
using ImpostorHqR.Core.Impostor.Events;
using ImpostorHqR.Core.Services;

namespace ImpostorHqR.Core.Command
{
    public class CommandListener : IService
    {
        public void PostInit()
        {
            ImpostorPlayerEventListener.Instance.PlayerChat += PlayerChat;
        }

        private void PlayerChat(IPlayerChatEvent obj)
        {
            if (obj.Message[0] == '/')
            {
                CommandProcessor.ProcessGameChat(obj.ClientPlayer, obj.Message);
            }
        }

        public void Activate() { }

        public void Shutdown() { }
    }
}
