using System.Collections.Generic;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage;

namespace ImpostorHqR.Core.Web.Page.Store
{
    public class WebPageStore
    {
        public static readonly WebPageStore Instance = new WebPageStore();

        public List<SimplePageHandler> SimplePages = new List<SimplePageHandler>();
        public List<SimpleApiPage> SimpleApiPages = new List<SimpleApiPage>();
    }
}
