using System.Drawing;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Console;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiConsolePage
{
    public class ApiConsolePageProvider : IReadonlyConsolePageProvider
    {
        public static readonly IReadonlyConsolePageProvider Instance = new ApiConsolePageProvider();

        public IReadonlyConsolePage ProduceApiPage(string title, Color boxColor, string placeholder, string handle)
        {
            return new ApiConsolePage(boxColor, title, handle, placeholder);
        }
    }
}
