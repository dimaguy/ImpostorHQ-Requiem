using Fleck;

namespace ImpostorHqR.Core.Web.Api.WebSockets
{
    public class HqApiIncomingMessage
    {
        public IWebSocketConnection Sender { get; set; }

        public string MessageData { get; set; }
    }
}
