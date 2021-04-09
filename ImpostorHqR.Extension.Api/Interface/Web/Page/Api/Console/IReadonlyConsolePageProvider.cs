using System.Drawing;

namespace ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Console
{
    public interface IReadonlyConsolePageProvider
    {
        public IReadonlyConsolePage ProduceApiPage(string title, Color boxColor, string placeholder, string handle);
    }
}
