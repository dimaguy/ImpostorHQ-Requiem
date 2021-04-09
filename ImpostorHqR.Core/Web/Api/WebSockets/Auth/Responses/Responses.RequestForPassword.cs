namespace ImpostorHqR.Core.Web.Api.WebSockets.Auth.Responses
{
    public class RequestForPasswordResponse : IHqAuthResponse
    {
        public string Serialize() => "RFP";
    }
}
