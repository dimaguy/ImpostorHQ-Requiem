using System;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Configuration;
using ImpostorHqR.Extension.Graphs.Events.WebPages;

namespace ImpostorHqR.Extension.Graphs.Events
{
    public class Start : IExtensionRequiem
    {
        public static WebPageConfig GetConfig() => IConfigurationStore.GetByType<WebPageConfig>()!;

        public static event Action OnClosed;

        public void Init()
        {
            if (GetConfig().EnableEventsPage)
            {
                Api.Registry.Impostor.EventManager.RegisterListener(BenchmarkEventListener.Instance);
                EventWebPage.Create();
            }

            ConsoleWebPage.Create();
        }

        public void PostInit()
        {
           
        }

        public void Shutdown()
        {
            OnClosed?.Invoke();
        }
    }
}
