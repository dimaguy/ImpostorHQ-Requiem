namespace ImpostorHqR.Core.Web.Api.WebSockets.Auth.Responses
{
    public class WelcomeResponse : IHqAuthResponse
    {
        public string Serialize() => "Welcome";
    }
}
