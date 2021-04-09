using System.Drawing;
using ImpostorHqR.Extension.Api.Interface.Web.Page.NoApi;

namespace ImpostorHqR.Core.Web.Page.Generator.NoApi
{
    public class SimplePageProvider : ISimplePageProvider
    {
        public static readonly ISimplePageProvider Instance = new SimplePageProvider();

        public ISimplePage ProduceTablePage(Color webColor, string title) => new TableSite.TableSite(webColor, title);
    }
}
