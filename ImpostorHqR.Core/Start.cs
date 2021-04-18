using System;
using System.Diagnostics;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games.Managers;
using Impostor.Api.Net.Manager;
using Impostor.Api.Net.Messages;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ImpostorHqR.Core.Command;
using ImpostorHqR.Core.Command.Included;
using ImpostorHqR.Core.Command.Processor;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Configuration.Loader;
using ImpostorHqR.Core.Extension.Loader;
using ImpostorHqR.Core.Extension.Loader.ServiceManager;
using ImpostorHqR.Core.Impostor;
using ImpostorHqR.Core.Impostor.Events;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.ObjectPool.StringBuilder;
using ImpostorHqR.Core.Web;
using ImpostorHqR.Core.Web.Api.WebSockets;
using ImpostorHqR.Core.Web.Http.Server;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Configuration;
using ImpostorHqR.Extension.Api.Service;

namespace ImpostorHqR.Core
{
    [ImpostorPlugin("ImpostorHqR.Core", "ImpostorHq Requiem", "the dima", "requiem")]
    public class ImpostorHqPlugin : PluginBase, IDimaCode
    {
        public ImpostorHqPlugin(ILogger<ImpostorHqPlugin> logger, IEventManager eventManager, IGameManager gameManager, IMessageWriterProvider messageWriterProvider, IClientManager clientManager)
        {
            ImpostorHqR.Extension.Api.Registry.Impostor.ClientManager = clientManager;
            ImpostorHqR.Extension.Api.Registry.Impostor.EventManager = eventManager;
            ImpostorHqR.Extension.Api.Registry.Impostor.GameManager = gameManager;
            ImpostorHqR.Extension.Api.Registry.Impostor.Logger = logger;
            ImpostorHqR.Extension.Api.Registry.Impostor.MessageWriterProvider = messageWriterProvider;

            ReusableStringBuilderPool.Initialize();
            LoggingManager.Initialize();
            ConfigurationLoader.LoadConfigs(typeof(ImpostorHqPlugin).Assembly);
            ServiceManager.Init();
            ImpostorEventListenerCreator.Initialize();
            CommandProcessor.Start();
            CommandListener.Start();
            HelpCommandRegister.Register();
            WebBinder.Bind();
            WebApiListener.Start();
            HttpServer.Start();
            ExtensionLoader.Initialize();
            ExtensionLoader.Start();
        }

        public override ValueTask EnableAsync()
        {
            ExtensionLoader.Store?.ExtensionPostInit(); //null if no extensions are loaded.
            return default;
        }

        public override ValueTask DisableAsync()
        {
            //////
            ConfigurationLoader.SafeConfigs();
            LoggingManager.Shutdown();
            WebApiListener.Shutdown();
            HttpServer.Shutdown();
            ExtensionLoader.Store?.ExtensionShutDown();
            return default;
        }
    }
}
