using ImpostorHq.Extension.Api.Loader;
using ImpostorHq.Extension.Api.Loader.Timeline;
using ImpostorHq.Extension.Test.Impostor;
using ImpostorHq.Extensions.Api.Interface.Export;
using ImpostorHq.Extensions.Api.Interface.Impostor;
using Microsoft.Extensions.Logging;

namespace ImpostorHq.Extension.Test
{
    public class Proxy : ExtensionProxy
    {
        public IImpostorObjectHolder Impostor => base.PreInitialization.ImpostorObjects;

        public static Proxy Instance { get; set; }

        public override void Init(IInitializationEvent instance)
        {
            Impostor.ImpostorLogger.LogInformation("Test extension init.");

            var boxSearch = instance.ComponentHub.Acquire("anti.ihq.test", "FastNumberBox");

            Impostor.ImpostorLogger.LogInformation($"Export bus box test number: {(boxSearch.Result as IPrimitiveBox)?.ReadLong()}");
            var message = instance.ComponentHub.Acquire("anti.ihq.test", "Message").Result as string;
            Impostor.ImpostorLogger.LogInformation($"Export bus simple test message: \"{message}\"");
        }

        public override void PostInit(IPostInitializationEvent instance)
        {
            Impostor.ImpostorLogger.LogInformation("Test extension post-init.");
        }

        public override void PreInit(IPreInitializationEvent instance)
        {


            Impostor.ImpostorLogger.LogInformation("Test extension pre-init.");
            Impostor.ImpostorEventManager.RegisterListener(new ImpostorEventListener());
            instance.ConsoleLogger.LogInformation($"Message extension config message: {ConfigHolder.Instance.Message}");

            instance.CommandApi.Processor.RegisterGameCommand(Commands.Commands.GetLoadCommand());
            instance.CommandApi.Processor.RegisterGameCommand(Commands.Commands.GetTestCommand());
        }


        public override void Shutdown()
        {
            Impostor.ImpostorLogger.LogInformation("Test extension shutdown.");
        }
    }
}
