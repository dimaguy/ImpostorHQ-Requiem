using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImpostorHqR.Extension.Api.Api.Web
{
    public interface IStaticPage
    {
        #region Provider

        private static Converter<(Color webColor, string title, string handle), IStaticPage> _Provider;
        private static Converter<(Color webColor, string title, string handle, WebPageAuthenticationOption credentials), IStaticPage> _SecureProvider;


        public static IStaticPage Create(Color webColor, string title, string handle)
        {
            return _Provider.Invoke((webColor, title, handle));
        }

        public static IStaticPage Create(Color webColor, string title, string handle, WebPageAuthenticationOption credentials)
        {
            return _SecureProvider.Invoke((webColor, title, handle, credentials));
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
