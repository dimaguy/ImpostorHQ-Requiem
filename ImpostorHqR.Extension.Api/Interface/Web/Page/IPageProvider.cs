using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Console;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Graph;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Simple;
using ImpostorHqR.Extension.Api.Interface.Web.Page.NoApi;

namespace ImpostorHqR.Extension.Api.Interface.Web.Page
{
    public interface IPageProvider
    {
        ISimplePageProvider SimplePageProvider { get; }

        IHtmlEntryAttributes SimplePageAttributeHelper { get; }


        ISimpleApiPageProvider SimpleApiPageProvider { get; }

        ISimpleApiPageElementProvider SimpleApiPageElementProvider { get; }

        IGraphPageProvider GraphPageProvider { get; }

        IGraphProvider GraphProvider { get; }

        IReadonlyConsolePageProvider ReadonlyConsolePageProvider { get; }
    }
}
