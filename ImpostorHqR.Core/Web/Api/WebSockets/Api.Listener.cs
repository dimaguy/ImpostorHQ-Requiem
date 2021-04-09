using Fleck;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Services;

namespace ImpostorHqR.Core.Web.Api.WebSockets
{
    public class HqApiListener : IService
    {
        public static HqApiListener Instance;

        public WebSocketServer Listener { get; private set; }

        public bool Running { get; private set; }

        public HqApiListener()
        {
            Instance = this;
        }

        public void PostInit()
        {
            this.Running = true;
            var location = $"ws://0.0.0.0:{ConfigHolder.Instance.ApiPort}";
            ConsoleLogging.Instance.LogInformation($"Starting API server at: {location}", true);
            this.Listener = new WebSocketServer(location);
            this.Listener.Start(socket =>
            {
                socket.OnOpen += () => HqApiProcessor.Instance.Process(socket);
            });
        }

        public void Shutdown()
        {
            this.Running = false;
        }

        public void Activate() { }
    }
}
