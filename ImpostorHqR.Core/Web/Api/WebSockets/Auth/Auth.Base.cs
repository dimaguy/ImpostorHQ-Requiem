using System.Text.Json;

namespace ImpostorHqR.Core.Web.Api.WebSockets.Auth
{
    public class HqAuthBaseMessage
    {
        public AuthStage Stage { get; set; }

        public string Data { get; set; }

        public static HqAuthBaseMessage Deserialize(string data)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<HqAuthBaseMessage>(data);
                return obj;
            }
            catch
            {
                return null;
            }
        }
    }

    public enum AuthStage
    {
        NegotiateHandle,
        RequestPassword,
        Authenticated
    }
}
