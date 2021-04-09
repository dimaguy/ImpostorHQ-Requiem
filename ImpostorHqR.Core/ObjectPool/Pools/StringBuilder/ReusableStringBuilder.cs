using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ImpostorHqR.Extension.Api.Interface.Helpers.ObjectPool.Included;

namespace ImpostorHqR.Core.ObjectPool.Pools.StringBuilder
{
    public struct ReusableStringBuilder : IReusableStringBuilder
    {
        public System.Text.StringBuilder StringBuilder { get; }

        public bool Used { get; private set; }

        public ReusableStringBuilder(System.Text.StringBuilder sb)
        {
            this.StringBuilder = sb;
            this.Used = false;
        }

        #region Overloads

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
        public void Dispose()
        {
            StringBuilderPool.Pool.Return(this.StringBuilder);
            this.Used = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => StringBuilder.ToString();
    }
}
