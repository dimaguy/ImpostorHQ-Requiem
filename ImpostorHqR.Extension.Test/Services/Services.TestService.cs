using ImpostorHq.Extension.Api.Interface;
using Microsoft.Extensions.Logging;

namespace ImpostorHq.Extension.Test.Services
{
    internal class TestService : IExtensionService
    {
        public void Init()
        {
            Proxy.Instance.Impostor.ImpostorLogger.LogInformation("Test extension service init.");

        }

        public void PostInit()
        {
            Proxy.Instance.Impostor.ImpostorLogger.LogInformation("Test extension service post-init.");
        }

        public void Shutdown()
        {
            Proxy.Instance.Impostor.ImpostorLogger.LogInformation("Test extension service stopping.");

        }
    }
}