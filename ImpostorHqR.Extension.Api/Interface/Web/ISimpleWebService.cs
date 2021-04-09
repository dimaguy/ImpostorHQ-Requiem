using ImpostorHqR.Extension.Api.Interface.Web.Page.NoApi;

namespace ImpostorHqR.Extension.Api.Interface.Web
{
    public interface ISimpleWebService
    {
        (string, ISimplePage) Register();

        ushort UpdateInterval { get; }

        void Update();
    }
}
