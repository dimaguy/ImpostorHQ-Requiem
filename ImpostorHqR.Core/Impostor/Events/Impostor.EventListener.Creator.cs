using ImpostorHqR.Core.Services;

namespace ImpostorHqR.Core.Impostor.Events
{
    public class ImpostorEventListenerCreator : IService
    {
        public void Activate()
        {
            var playerEventListener = new ImpostorPlayerEventListener();
            var gameEventListener = new ImpostorGameEventListener();
            ImpostorObjectHolder.Instance.ImpostorEventManager.RegisterListener(playerEventListener);
            ImpostorObjectHolder.Instance.ImpostorEventManager.RegisterListener(gameEventListener);
        }

        public void Shutdown() { }

        public void PostInit() { }
    }
}
