using System;

namespace ImpostorHqR.Extension.Api
{
    public interface IReusableStringBuilder : IDisposable
    {
        public static Func<IReusableStringBuilder> Get;

        System.Text.StringBuilder StringBuilder { get; }

        void Append(char c);

        void Append(string chars);

        void Append(char[] chars);

        void Append(ReadOnlySpan<char> chars);

        void Append(ReadOnlyMemory<char> chars);

        void AppendLine(string chars);

        void AppendLine(char[] chars);

        void AppendLine(ReadOnlySpan<char> chars);

        void AppendLine(ReadOnlyMemory<char> chars);

        void Clear();
    }
}
