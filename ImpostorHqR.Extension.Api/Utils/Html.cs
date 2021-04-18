using System.Drawing;

namespace ImpostorHqR.Extension.Api.Utils
{
    public class HtmlHelper
    {

        public static string SetColorAttribute(string content, Color color)
        {

            return "<p style=\"color: rgb(" + $"{color.R}, {color.G}, {color.B}" + ")\">" + content + "</p>";
        }

        public static string SetSizeAttribute(string content, byte size)
        {
            return "<p style=\"font-size:" + size + "px\">" + content + "</p>";
        }
    }
}
