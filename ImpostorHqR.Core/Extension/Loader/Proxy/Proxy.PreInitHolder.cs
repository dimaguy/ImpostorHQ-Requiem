using ImpostorHqR.Extension.Api.Interface.Helpers.ObjectPool;
using ImpostorHqR.Core.Command.Processor;
using ImpostorHqR.Core.Extension.ComponentBus.Primitive;
using ImpostorHqR.Core.Extension.Helper;
using ImpostorHqR.Core.Impostor;
using ImpostorHqR.Core.Impostor.Chat;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.ObjectPool.Pools.StringBuilder;
using ImpostorHqR.Extension.Api.Interface.Helpers;
using ImpostorHqR.Extension.Api.Interface.Helpers.ObjectPool.Included;
using ImpostorHqR.Extension.Api.Interface.Web.Page;
using ImpostorHqR.Extension.Api.Loader.Timeline;
using ImpostorHqR.Extensions.Api.Interface.Export;
using ImpostorHqR.Extensions.Api.Interface.Game;
using ImpostorHqR.Extensions.Api.Interface.Impostor;
using ImpostorHqR.Extensions.Api.Interface.Logging;

namespace ImpostorHqR.Core.Extension.Loader.Proxy
{
    internal class PreInitHolder : IPreInitializationEvent
    {
        public IImpostorObjectHolder ImpostorObjects => ImpostorObjectHolder.Instance;

        public IConsoleLogger ConsoleLogger => ConsoleLogging.Instance;

        public ILogManager LogManager => Logging.LogManager.Instance;

        public ICommandApi CommandApi => CommandApiHolder.Instance;

        public IChatUtils ChatUtils => ChatHolder.Instance;

        public IPrimitiveBoxProvider PrimitiveBoxProvider => PrimitiveBoxProducer.Instance;

        public IPageProvider PageProvider => Web.Common.PageProvider.Instance;

        public IServerLoadMonitor LoadMonitor => ServerLoadMonitor.Instance;

        IReusableStringBuilderPool IPreInitializationEvent.StringBuilderPool => StringBuilderPool.Instance;
    }
}
