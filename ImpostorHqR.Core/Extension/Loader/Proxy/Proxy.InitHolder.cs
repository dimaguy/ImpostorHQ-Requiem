using ImpostorHqR.Core.Extension.ComponentBus;
using ImpostorHqR.Core.Web.Page.Store;
using ImpostorHqR.Extension.Api.Interface.Web;
using ImpostorHqR.Extension.Api.Loader.Timeline;
using ImpostorHqR.Extensions.Api.Interface.Export;

namespace ImpostorHqR.Core.Extension.Loader.Proxy
{
    internal class InitHolder : IInitializationEvent
    {
        public IComponentHub ComponentHub => ComponentBusManager.Instance;

        public IPageRegistry PageRegistry => WebPageRegistry.Instance;
    }
}
