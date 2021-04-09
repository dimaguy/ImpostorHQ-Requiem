using System.Collections.Generic;
using System.Drawing;

namespace ImpostorHqR.Extension.Api.Interface.Web.Page.NoApi
{
    public interface ISimplePage
    {
        bool Update { get; }

        void AddEntry(string text);

        void RemoveEntry(string text);

        IEnumerable<string> Entries { get; }

        Color Color { get; }

        string Title { get; }

        void Clear();

        void SetParameters(Color color, string title);
    }
}
