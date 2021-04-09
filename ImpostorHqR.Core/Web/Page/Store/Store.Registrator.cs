using System.Linq;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Core.Web.Page.Generator.NoApi.TableSite;
using ImpostorHqR.Extension.Api.Interface.Web;
using ImpostorHqR.Extension.Api.Interface.Web.Page.NoApi;

namespace ImpostorHqR.Core.Web.Page.Store
{
    public class WebPageRegistry : IPageRegistry
    {
        public static WebPageRegistry Instance = new WebPageRegistry();

        public void RegisterSimplePage(string urlSuffix, ISimplePage page)
        {
            if (!(page is TableSite site)) return;

            lock (WebPageStore.Instance.SimplePages)
            {
                if ((WebPageStore.Instance.SimplePages.FirstOrDefault(c => c.Handle.Path.Equals(urlSuffix))) != null)
                {
                    ConsoleLogging.Instance.LogError($"Web page already exists: {urlSuffix}. A second one will not be registered.", this);
                    return;
                };
                WebPageStore.Instance.SimplePages.Add(new SimplePageHandler(urlSuffix, (TableSite)page));
            }

        }
    }
}
