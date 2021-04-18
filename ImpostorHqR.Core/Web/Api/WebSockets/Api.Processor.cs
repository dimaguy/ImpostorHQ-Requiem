using Fleck;
using System.Collections.Generic;
using System.Linq;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Api.WebSockets.Auth;
using ImpostorHqR.Core.Web.Api.WebSockets.Auth.Responses;
using ImpostorHqR.Core.Web.Api.WebSockets.Handles;
using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Core.Web.Api.WebSockets
{
    public static class HqApiProcessor
    {
        private static readonly List<HqApiPreClient> _timeoutQueue = new List<HqApiPreClient>();

        static HqApiProcessor()
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
            lock (_timeoutQueue)
            {
                if (_timeoutQueue.Count > 0)
                {
                    for (var i = 0; i < _timeoutQueue.Count; i++)
                    {
                        var client = _timeoutQueue[i];
                        if (client.Cycles < HqApiProcessorConstant.AuthTimeoutSeconds) client.Cycles++;
                        else
                        {
                            client.TimedOut = true;
                            client.Connection.Close();
                            _timeoutQueue.RemoveAt(i);

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
            lock (_timeoutQueue) _timeoutQueue.Add(record);
            client.OnMessage += (message) => OnMessage(message, record);
        }

        private static void OnMessage(string message, HqApiPreClient client)
        {
            if (client.Stage != AuthStage.Authenticated)
            {
                lock (_timeoutQueue)
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
                lock (_timeoutQueue)
                {
                    if (_timeoutQueue.Contains(client)) _timeoutQueue.Remove(client);
                    if (fail) client.Connection.Close();
                }
            }
        }
    }
}
