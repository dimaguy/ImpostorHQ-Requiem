using System.Drawing;
using ImpostorHqR.Extension.Api.Interface.Web;

namespace ImpostorHqR.Core.Web.Page.Generator.Helper
{
    public class HtmlHelper : IHtmlEntryAttributes
    {
        public static readonly IHtmlEntryAttributes Instance = new HtmlHelper();

        public string SetColorAttribute(string content, Color color)
        {

            return "<p style=\"color: rgb(" + $"{color.R}, {color.G}, {color.B}" + ")\">" + content + "</p>";
        }

        public string SetSizeAttribute(string content, byte size)
        {
            return "<p style=\"font-size:" + size + "px\">" + content + "</p>";
        }
    }
}
