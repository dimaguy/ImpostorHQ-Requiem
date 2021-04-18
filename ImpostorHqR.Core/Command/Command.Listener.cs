using Impostor.Api.Events.Player;
using ImpostorHqR.Core.Command.Processor;
using ImpostorHqR.Core.Impostor.Events;
using ImpostorHqR.Extension.Api.Service;

namespace ImpostorHqR.Core.Command
{
    public static class CommandListener
    {
        public static void Start()
        {
            IServiceManager.GetSingleton<ImpostorPlayerEventListener>().PlayerChat += PlayerChat;
        }

        private static void PlayerChat(IPlayerChatEvent obj)
        {
            if (obj.Message[0] == '/')
            {
                CommandProcessor.ProcessGameChat(obj.ClientPlayer, obj.Message);
            }
        }
    }
}
