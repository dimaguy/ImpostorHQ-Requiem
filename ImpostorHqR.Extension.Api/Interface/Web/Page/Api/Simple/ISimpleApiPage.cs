using System.Drawing;

namespace ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Simple
{
    public interface ISimpleApiPage
    {
        string Title { get; }

        string Handle { get; }

        Color ElementColor { get; }

        void Push(ISimpleApiPageElement element);

        void Clear();

        void Set(string text, Color color);
    }
}
