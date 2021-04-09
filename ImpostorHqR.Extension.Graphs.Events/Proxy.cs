using ImpostorHqR.Extension.Api.Loader;
using ImpostorHqR.Extension.Api.Loader.Timeline;
using ImpostorHqR.Extension.Graphs.Events.WebPages;

namespace ImpostorHqR.Extension.Graphs.Events
{
    public class Proxy : ExtensionProxy
    {
        public static Proxy Instance;

        public override void PreInit(IPreInitializationEvent instance)
        {
            if (WebPageConfig.Instance.EnableEventsPage)
            {
                instance.ImpostorObjects.ImpostorEventManager.RegisterListener(BenchmarkEventListener.Instance);
            }
        }

        public override void Init(IInitializationEvent instance)
        {
        }

        public override void PostInit(IPostInitializationEvent instance)
        {
        }

        public override void Shutdown()
        {
        }
    }
}
