namespace ImpostorHqR.Core.Web.Api.WebSockets.Auth.Responses
{
    public class HandleNotFoundResponse : IHqAuthResponse
    {
        public string Serialize() => "HNF";
    }
}
