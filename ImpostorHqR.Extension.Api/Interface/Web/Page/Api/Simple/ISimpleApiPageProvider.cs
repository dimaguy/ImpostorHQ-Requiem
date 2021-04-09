using System.Drawing;

namespace ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Simple
{
    public interface ISimpleApiPageProvider
    {
        public ISimpleApiPage ProduceApiPage(string title, Color elementColor, string handle);
    }
}
