using System.Drawing;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Simple;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage
{
    public class SimpleApiPageProvider : ISimpleApiPageProvider
    {
        public static readonly ISimpleApiPageProvider Instance = new SimpleApiPageProvider();

        public ISimpleApiPage ProduceApiPage(string title, Color elementColor, string handle)
        {
            return new SimpleApiPage(elementColor, title, handle);
        }
    }
}
