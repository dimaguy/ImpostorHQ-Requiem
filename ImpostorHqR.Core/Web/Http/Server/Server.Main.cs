using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using HqResearch.Ssl;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Api.WebSockets.Handles;
using ImpostorHqR.Core.Web.Common.Protection.DoS;
using ImpostorHqR.Core.Web.Http.Server.Client;
using ImpostorHqR.Core.Web.Http.Server.Response;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Core.Web.Http.Server
{
    //TODO: INSTANCE
    static class HttpServer
    {
        public static X509Certificate2 Ssl = SslCertificateProvider.GetCertificate("anti.the-dying-of-the-light.who");

        private static bool Cancel { get; set; }

        private static Socket Listener { get; set; }

        public static void Start()
        {
            HttpClientProcessor.Initialize();
            HttpErrorResponses.Initialize();
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Static;
            typeof(IServerLoad).GetProperty("GetHttpActiveThreads", flags)!.SetValue(null, new Func<int>(HttpClientWorkPool.GetActiveThreads));
            typeof(IServerLoad).GetProperty("GetHttpRequestRate", flags)!.SetValue(null, new Func<int>(() => HttpClientWorkPool.RequestsPerSecond));
            typeof(IServerLoad).GetProperty("GetHttpRateKbPerSecond", flags)!.SetValue(null, new Func<int>(() => HttpClientWorkPool.FileDataRateKbPerSecond));
            typeof(IServerLoad).GetProperty("GetCacheSizeKb", flags)!.SetValue(null, new Func<int>(() => HttpServerFileCache.CurrentSize));
            typeof(IServerLoad).GetProperty("GetApiUsersCount", flags)!.SetValue(null, new Func<int>(WebApiHandleStore.GetClientCount));

            ILogManager.Log($"Starting HTTP{(IConfigurationStore.GetByType<RequiemConfig>().EnableSsl ? "S" : "")} server on {IConfigurationStore.GetByType<RequiemConfig>().HttpPort}.", "HTTP Server", LogType.Information);
            Listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Listener.Bind(new IPEndPoint(IPAddress.Any, IConfigurationStore.GetByType<RequiemConfig>().HttpPort));
            Listener.Listen();
            HqServerProtector.OnBlocked += OnClientBlocked;

            var listenArgs = new SocketAsyncEventArgs();
            listenArgs.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptClient);

            Listener.AcceptAsync(listenArgs);
        }

        private static void OnClientBlocked(IPAddress obj)
        {
            ILogManager.Log($"Rate-limited connection attempt from {obj}!", "HTTP Server", LogType.Warning);
        }

        private static void AcceptClient(object sender, SocketAsyncEventArgs listenEventArgs)
        {
            if (Cancel) return;
            var socket = listenEventArgs.AcceptSocket;
            var listen = (Socket) sender;
            listenEventArgs.AcceptSocket = null;

            Trace.Assert(socket!=null, "Listen socket null! Server.Main L65");

            listen.AcceptAsync(listenEventArgs);

            if (HqServerProtector.IsAttacking(((IPEndPoint)socket.RemoteEndPoint)?.Address))
            {
                socket.Close();
                return;
            }

            _ = HttpClientWorkPool.ProcessWorkItemAsync(socket);
        }

        public static void Shutdown()
        {
            Cancel = true;
            Listener.Close();
        }
    }
}
