using System;
using System.Drawing;

namespace ImpostorHqR.Extension.Api.Api.Web
{
    public interface IApiPage
    {
        #region Provider

        private static Converter<(string, Color, string), IApiPage> _Provider;

        public static IApiPage Create(string title, Color elementColor, string handle)
        {
            return _Provider.Invoke((title, elementColor, handle));
        }

        #endregion

        string Title { get; }

        string Handle { get; }

        Color ElementColor { get; }

        void Push(ApiPageElement element);

        void Clear();

        void Set(string text, Color color);
    }
}
