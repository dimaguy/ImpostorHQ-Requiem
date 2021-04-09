using ImpostorHqR.Core.Web.Page.Generator.Api.ApiConsolePage;
using ImpostorHqR.Core.Web.Page.Generator.Api.ApiGraphPage;
using ImpostorHqR.Core.Web.Page.Generator.Helper;
using ImpostorHqR.Extension.Api.Interface.Web;
using ImpostorHqR.Extension.Api.Interface.Web.Page;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Console;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Graph;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Simple;
using ImpostorHqR.Extension.Api.Interface.Web.Page.NoApi;

namespace ImpostorHqR.Core.Web.Common
{
    public class PageProvider : IPageProvider
    {
        public static readonly IPageProvider Instance = new PageProvider();

        public ISimplePageProvider SimplePageProvider => Page.Generator.NoApi.SimplePageProvider.Instance;

        public IHtmlEntryAttributes SimplePageAttributeHelper => HtmlHelper.Instance;

        public ISimpleApiPageProvider SimpleApiPageProvider => Page.Generator.Api.ApiTablePage.SimpleApiPageProvider.Instance;

        public ISimpleApiPageElementProvider SimpleApiPageElementProvider => Page.Generator.Api.ApiTablePage.SimpleApiPageElementProvider.Instance;

        public IGraphPageProvider GraphPageProvider => ApiGraphPageProvider.Instance;

        public IGraphProvider GraphProvider => ApiGraphPageElementProvider.Instance;

        public IReadonlyConsolePageProvider ReadonlyConsolePageProvider => ApiConsolePageProvider.Instance;
    }
}
