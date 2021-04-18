using Fleck;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Core.Web.Api.WebSockets
{
    public static class WebApiListener
    {
        public static WebSocketServer Listener { get; private set; }

        public static bool Running { get; private set; }

        public static void Start()
        {
            HqApiProcessor.Initialize();
            Running = true;
            var location = $"ws://0.0.0.0:{IConfigurationStore.GetByType<RequiemConfig>().ApiPort}";
            Listener = new WebSocketServer(location);
            Listener.Start(socket =>
            {
                socket.OnOpen += () => HqApiProcessor.Process(socket);
            });
        }

        public static void Shutdown()
        {
            Running = false;
            Listener.Dispose();
        }
    }
}
