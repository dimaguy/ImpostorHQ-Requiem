using ImpostorHqR.Core.Web.Api.WebSockets;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage
{
    public struct SimpleApiPageClearMessage : IHqApiOutgoingMessage
    {
        public string Serialize()
        {
            return "{\"Clear\":true}";
        }
    }
}
