using System.Drawing;

namespace ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Simple
{
    public interface ISimpleApiPageElementProvider
    {
        ISimpleApiPageElement Create(string text, Color backgroundColor);
    }
}
