using System.Drawing;

namespace ImpostorHqR.Extension.Api.Interface.Web.Page.NoApi
{
    public interface ISimplePageProvider
    {
        public ISimplePage ProduceTablePage(Color webColor, string title);
    }
}
