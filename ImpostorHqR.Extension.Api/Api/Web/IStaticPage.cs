using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImpostorHqR.Extension.Api.Api.Web
{
    public interface IStaticPage
    {
        #region Provider

        private static Converter<(Color webColor, string title), IStaticPage> _Provider;

        public static IStaticPage Create(Color webColor, string title)
        {
            return _Provider.Invoke((webColor, title));
        }

        #endregion

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
