
using ImpostorHqR.Extension.Api.Service;

namespace ImpostorHqR.Core.Impostor.Events
{
    public static class ImpostorEventListenerCreator
    {
        public static void Initialize()
        {
            var playerEventListener = new ImpostorPlayerEventListener();
            var gameEventListener = new ImpostorGameEventListener();
            ImpostorHqR.Extension.Api.Registry.Impostor.EventManager.RegisterListener(playerEventListener);
            ImpostorHqR.Extension.Api.Registry.Impostor.EventManager.RegisterListener(gameEventListener);
            IServiceManager.RegisterSingleton(playerEventListener);
            IServiceManager.RegisterSingleton(gameEventListener);
        }
    }
}
