using Fleck;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using ImpostorHqR.Core.Configuration;
using ImpostorHqR.Core.Configuration.Loader;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Api.WebSockets.Auth;
using ImpostorHqR.Core.Web.Api.WebSockets.Auth.Responses;
using ImpostorHqR.Core.Web.Api.WebSockets.Handles;
using ImpostorHqR.Core.Web.Http.Server.Client;
using ImpostorHqR.Extension.Api;
using ImpostorHqR.Extension.Api.Configuration;

namespace ImpostorHqR.Core.Web.Api.WebSockets
{
    public static class HqApiProcessor
    {
        private static readonly List<HqApiPreClient> TimeoutQueue = new List<HqApiPreClient>();

        private static readonly RequiemConfig Cfg = IConfigurationStore.GetByType<RequiemConfig>();

        public static void Initialize()
        {
            var tmr = new System.Timers.Timer(1000)
            {
                AutoReset = true,
                Enabled = true
            };
            tmr.Elapsed += RemoveCallback;
            tmr.Start();
        }

        private static void RemoveCallback(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (TimeoutQueue)
            {
                if (TimeoutQueue.Count > 0)
                {
                    for (var i = 0; i < TimeoutQueue.Count; i++)
                    {
                        var client = TimeoutQueue[i];
                        if (client.Cycles < Cfg.ApiAuthTimeoutSeconds) client.Cycles++;
                        else
                        {
                            client.TimedOut = true;
                            client.Connection.Close();
                            TimeoutQueue.RemoveAt(i);

                            ILogManager.Log($"Client {client.Connection.ConnectionInfo.ClientIpAddress} timed out on authentication.", "Api.Processor", LogType.Warning);
                        }
                    }
                }
            }
        }

        public static void Process(IWebSocketConnection client)
        {
            var record = new HqApiPreClient()
            {
                Connection = client,
                Cycles = 0,
                TimedOut = false,
                Stage = AuthStage.NegotiateHandle
            };
            lock (TimeoutQueue) TimeoutQueue.Add(record);
            client.OnMessage += (message) => OnMessage(message, record);
        }

        private static void OnMessage(string message, HqApiPreClient client)
        {
            if (client.Stage != AuthStage.Authenticated)
            {
                lock (TimeoutQueue)
                {
                    if (!client.TimedOut)
                    {
                        HandleAuthentication(message, client);
                    }
                }
            }
            else
            {
                client.User.Received(message);
            }
        }

        private static void HandleAuthentication(string message, HqApiPreClient client)
        {
            if (client.Stage != AuthStage.NegotiateHandle) return;
            var request = HqAuthBaseMessage.Deserialize(message);
            if (request == null)
            {
                Remove(client, true);
                return;
            }

            if (request.Stage != AuthStage.NegotiateHandle)
            {
                // intruder.
                Remove(client, true);
                return;
            }
            var data = HqApiAuthRequest.Deserialize(request.Data);

            if (data == null)
            {
                Remove(client, true);
                return;
            }
            var handle = WebApiHandleStore.Handles.FirstOrDefault(item =>
                item.HandleId.Equals(data.Handle) && item.Secure == data.Secure);

            if (handle == null)
            {
                client.Push(new HandleNotFoundResponse());
                Remove(client, true);
                return;
            }

            if (handle.Secure)
            {
                client.Push(new RequestForPasswordResponse());
                client.Stage = AuthStage.RequestPassword;
            }
            else
            {
                //get socket from fleck...
                var socket = ((SocketWrapper) (((WebSocketConnection) client.Connection).Socket))._socket;
                Trace.Assert(socket.RemoteEndPoint != null, "Socket remote end point null after fleck cast!");
                if (!HttpClientProcessor.IsAuthorized(handle.HandleId, ((IPEndPoint)socket.RemoteEndPoint).Address.MapToIPv6()))
                {
                    Remove(client, false);
                    client.Push(new UnauthorizedResponse());
                    return;
                }

                Remove(client, false);
                client.Push(new WelcomeResponse());
                client.Handle = handle;
                var user = new HqApiUser(client.Connection);
                handle.Add(user);
                client.User = user;
                client.Stage = AuthStage.Authenticated;
            }

            static void Remove(HqApiPreClient client, bool fail)
            {
                lock (TimeoutQueue)
                {
                    if (TimeoutQueue.Contains(client)) TimeoutQueue.Remove(client);
                    if (fail) client.Connection.Close();
                }
            }
        }
    }
}
