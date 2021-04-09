using ImpostorHqR.Extension.Api.Interface.Web;
using ImpostorHqR.Extensions.Api.Interface.Export;

namespace ImpostorHqR.Extension.Api.Loader.Timeline
{
    public interface IInitializationEvent
    {
        IComponentHub ComponentHub { get; }

        IPageRegistry PageRegistry { get; }
    }
}
