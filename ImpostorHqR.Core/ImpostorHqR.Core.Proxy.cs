using Impostor.Api.Events.Managers;
using Impostor.Api.Games.Managers;
using Impostor.Api.Net.Manager;
using Impostor.Api.Net.Messages;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Impostor;
using ImpostorHqR.Core.Services;
using ImpostorHqR.Extensions.Api.Interface.Logging;

namespace ImpostorHqR.Core
{
    [ImpostorPlugin("ImpostorHqR.Core", "ImpostorHq Requiem", "the dima", "requiem")]
    public class ImpostorHqPlugin : PluginBase, ILogEmitter
    {
        public static ImpostorHqPlugin Instance;

        public ImpostorHqPlugin(ILogger<ImpostorHqPlugin> logger, IEventManager eventManager, IGameManager gameManager, IMessageWriterProvider messageWriterProvider, IClientManager clientManager)
        {
            Instance = this;
            ImpostorObjectHolder.Instance.ImpostorClientManager = clientManager;
            ImpostorObjectHolder.Instance.ImpostorEventManager = eventManager;
            ImpostorObjectHolder.Instance.ImpostorGameManager = gameManager;
            ImpostorObjectHolder.Instance.ImpostorLogger = logger;
            ImpostorObjectHolder.Instance.ImpostorMessageWriterProvider = messageWriterProvider;
            ConfigHolder.Instance.Start();
            ServiceManager.Instance.PreInit();
        }

        public override ValueTask EnableAsync()
        {
            ServiceManager.Instance.Start();
            return default;
        }

        public override ValueTask DisableAsync()
        {
            ServiceManager.Instance.Stop();
            ConfigHolder.Instance.Shutdown();
            return default;
        }
    }
}
