using Fleck;
using ImpostorHqR.Core.Web.Api.WebSockets.Auth;
using ImpostorHqR.Core.Web.Api.WebSockets.Handles;

namespace ImpostorHqR.Core.Web.Api.WebSockets
{
    public struct HqApiPreClient
    {
        public HqApiUser User { get; set; }

        public ApiHandleHolder Handle { get; set; }

        public IWebSocketConnection Connection { get; set; }

        public bool Connected { get; private set; }

        public byte Cycles { get; set; }

        public bool TimedOut { get; set; }

        public AuthStage Stage { get; set; }

        public void Push(IHqAuthResponse response)
        {
            try
            {
                Connection.Send(response.Serialize());
            }
            catch
            {
                this.Connected = false;
            }
        }
    }
}
