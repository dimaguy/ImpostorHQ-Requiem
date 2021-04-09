using System.Drawing;
using ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Simple;

namespace ImpostorHqR.Core.Web.Page.Generator.Api.ApiTablePage
{
    public class SimpleApiPageElementProvider : ISimpleApiPageElementProvider
    {
        public static readonly ISimpleApiPageElementProvider Instance = new SimpleApiPageElementProvider();

        public ISimpleApiPageElement Create(string text, Color backgroundColor)
        {
            return new SimpleApiPageElement(text, backgroundColor);
        }
    }
}
