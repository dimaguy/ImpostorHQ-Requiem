using Fleck;
using System;
using System.Threading.Tasks;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extension.Api.Interface.Logging;
using ImpostorHqR.Extensions.Api.Interface.Logging;

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

        public async Task Send(string data)
        {
            try
            {
                await Connection.Send(data);
            }
            catch (Fleck.WebSocketException)
            {
                closed();
            }
            catch (Fleck.ConnectionNotAvailableException)
            {
                closed();
            }
            catch (Exception ex)
            {
                await LogManager.Instance.Log(new LogEntry()
                {
                    Message = $"Error in API user send: {ex}",
                    Source = this,
                    Type = LogType.Error
                });
            }

            void closed()
            {
                OnDisconnected?.Invoke(this);
                this.Connected = false;
            }
        }

        public void Received(string message)
        {
            OnReceive?.Invoke(this, message);
        }

        public event Action<HqApiUser> OnDisconnected;

        public event Action<HqApiUser, string> OnReceive;
    }
}