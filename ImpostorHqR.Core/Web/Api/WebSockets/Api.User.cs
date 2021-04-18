using Fleck;
using System;
using System.Threading.Tasks;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extension.Api;
using Microsoft.Extensions.Logging.Abstractions;

namespace ImpostorHqR.Core.Web.Api.WebSockets
{
    public class HqApiUser
    {
        public IWebSocketConnection Connection { get; }

        public bool Connected { get; private set; }

        public HqApiUser(IWebSocketConnection connection)
        {
            this.Connection = connection;
            this.Connected = true;
            connection.OnClose += () => OnDisconnected?.Invoke(this);
        }

        public async ValueTask Send(string data)
        {
            try
            {
                await Connection.Send(data);
            }
            catch (WebSocketException)
            {
                Closed();
            }
            catch (ConnectionNotAvailableException)
            {
                Closed();
            }
            catch (Exception ex)
            {
                ILogManager.Log($"Error in API Send!", this.ToString(), LogType.Error, true, true, ex);
            }

            void Closed()
            {
                OnDisconnected?.Invoke(this);
                this.Connected = false;
            }
        }

        public void Received(string message) => OnReceive?.Invoke(this, message);

        public event Action<HqApiUser> OnDisconnected;

        public event Action<HqApiUser, string> OnReceive;
    }
}