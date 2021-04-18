using ImpostorHqR.Extension.Api;

using ImpostorHqR.Extension.Graphs.Load.WebPages;

namespace ImpostorHqR.Extension.Graphs.Load
{
    public class Start : IExtensionRequiem
    {
        public void Init()
        {
            new WebPageExceptions().Start();
            new WebPageLoad().Start();
            new WebPageThreads().Start();
        }

        public void PostInit()
        {
           
        }

        public void Shutdown()
        {

        }
    }
}
