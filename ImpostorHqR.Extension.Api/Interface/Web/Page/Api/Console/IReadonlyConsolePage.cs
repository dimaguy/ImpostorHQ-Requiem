using System.Drawing;

namespace ImpostorHqR.Extension.Api.Interface.Web.Page.Api.Console
{
    public interface IReadonlyConsolePage
    {
        string Title { get; }

        string Placeholder { get; }

        string Handle { get; }

        Color BoxColor { get; }

        void Push(string text);

        void Clear();

        void Set(string text);
    }
}
