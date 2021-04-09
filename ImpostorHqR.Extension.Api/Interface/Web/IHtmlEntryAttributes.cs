using System.Drawing;

namespace ImpostorHqR.Extension.Api.Interface.Web
{
    public interface IHtmlEntryAttributes
    {
        string SetSizeAttribute(string content, byte size);

        string SetColorAttribute(string content, Color color);
    }
}
