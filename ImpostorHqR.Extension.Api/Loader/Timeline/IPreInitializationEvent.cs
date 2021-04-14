using ImpostorHqR.Extension.Api.Interface.Helpers.ObjectPool;
using ImpostorHqR.Extension.Api.Interface.Helpers;
using ImpostorHqR.Extension.Api.Interface.Helpers.ObjectPool.Included;
using ImpostorHqR.Extension.Api.Interface.Logging;
using ImpostorHqR.Extension.Api.Interface.Web.Page;
using ImpostorHqR.Extensions.Api.Interface.Export;
using ImpostorHqR.Extensions.Api.Interface.Game;
using ImpostorHqR.Extensions.Api.Interface.Impostor;
using ImpostorHqR.Extensions.Api.Interface.Logging;

namespace ImpostorHqR.Extension.Api.Loader.Timeline
{
    public interface IPreInitializationEvent
    {
        IImpostorObjectHolder ImpostorObjects { get; }

        IConsoleLogger ConsoleLogger { get; }

        ILogManager LogManager { get; }

        ICommandApi CommandApi { get; }

        IChatUtils ChatUtils { get; }

        IPrimitiveBoxProvider PrimitiveBoxProvider { get; }

        IPageProvider PageProvider { get; }

        IServerLoadMonitor LoadMonitor { get; }

        IReusableStringBuilderPool StringBuilderPool { get; }
    }
}
