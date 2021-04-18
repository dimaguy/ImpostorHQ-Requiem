using System;
using System.Runtime.CompilerServices;
using ImpostorHqR.Extension.Api;

namespace ImpostorHqR.Core.ObjectPool.StringBuilder
{
    public class ReusableStringBuilder : IReusableStringBuilder
    {
        public System.Text.StringBuilder StringBuilder { get; }

        public ReusableStringBuilder(System.Text.StringBuilder sb)
        {
            this.StringBuilder = sb;
        }

        #region Overloads

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char c) => StringBuilder.Append(c);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(string chars) => StringBuilder.Append(chars);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char[] chars) => StringBuilder.Append(chars);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(ReadOnlySpan<char> chars) => StringBuilder.Append(chars);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(ReadOnlyMemory<char> chars) => StringBuilder.Append(chars);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLine(string chars) => StringBuilder.AppendLine(chars);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLine(char[] chars)
        {
            StringBuilder.Append(chars);
            StringBuilder.AppendLine();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLine(ReadOnlySpan<char> chars)
        {
            StringBuilder.Append(chars);
            StringBuilder.AppendLine();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AppendLine(ReadOnlyMemory<char> chars)
        {
            StringBuilder.Append(chars);
            StringBuilder.AppendLine();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => this.StringBuilder.Clear();

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => ReusableStringBuilderPool.Return(this);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => StringBuilder.ToString();
    }
}
