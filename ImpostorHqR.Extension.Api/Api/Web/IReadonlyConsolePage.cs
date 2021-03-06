using System;
using System.Drawing;

namespace ImpostorHqR.Extension.Api.Api.Web
{
    public interface IReadonlyConsolePage
    {
        #region Provider

        private static Converter<(string, Color, string, string), IReadonlyConsolePage> _Provider;
        private static Converter<(string, Color, string, string, WebPageAuthenticationOption), IReadonlyConsolePage> _SecureProvider;

        public static IReadonlyConsolePage Create(string title, Color boxColor, string placeholder, string handle)
        {
            return _Provider.Invoke((title, boxColor, placeholder, handle));
        }

        public static IReadonlyConsolePage Create(string title, Color boxColor, string placeholder, string handle, WebPageAuthenticationOption credentials)
        {
            return _SecureProvider.Invoke((title, boxColor, placeholder, handle, credentials));
        }

        #endregion

        string Title { get; }

        string Placeholder { get; }

        string Handle { get; }

        Color BoxColor { get; }

        void Push(string text);

        void Clear();

        void Set(string text);
    }
}
