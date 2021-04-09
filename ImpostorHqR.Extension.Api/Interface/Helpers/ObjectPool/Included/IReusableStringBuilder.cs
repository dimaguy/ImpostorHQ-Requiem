using System;
using System.Text;

namespace ImpostorHqR.Extension.Api.Interface.Helpers.ObjectPool.Included
{
    public interface IReusableStringBuilder : IDisposable
    {
        StringBuilder StringBuilder { get; }

        public bool Used { get; }

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
