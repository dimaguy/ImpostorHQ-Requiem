using System;
using System.Text.Json;

namespace ImpostorHqR.Core.Web.Api.WebSockets.Auth
{
    [Serializable]
    public class HqApiAuthRequest
    {
        public string Handle { get; set; }

        public bool Secure { get; set; }

        public static HqApiAuthRequest Deserialize(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            try
            {
                return JsonSerializer.Deserialize<HqApiAuthRequest>(data);
            }
            catch
            {
                return null;
            }
        }
    }
}
