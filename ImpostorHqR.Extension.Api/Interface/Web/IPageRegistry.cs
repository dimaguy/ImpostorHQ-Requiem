using ImpostorHqR.Extension.Api.Interface.Web.Page.NoApi;

namespace ImpostorHqR.Extension.Api.Interface.Web
{
    public interface IPageRegistry
    {
        void RegisterSimplePage(string name, ISimplePage page);
    }
}
