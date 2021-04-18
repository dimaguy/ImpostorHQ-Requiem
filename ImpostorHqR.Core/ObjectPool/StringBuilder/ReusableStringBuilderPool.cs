using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using ImpostorHqR.Extension.Api;
using Microsoft.Extensions.ObjectPool;

namespace ImpostorHqR.Core.ObjectPool.StringBuilder
{
    public static class ReusableStringBuilderPool
    {
        private static readonly Microsoft.Extensions.ObjectPool.ObjectPool<ReusableStringBuilder> Pool =
            new DefaultObjectPool<ReusableStringBuilder>(new ReusableStringBuilderPoolPolicy());

        public static void Initialize()
        {
            typeof(IReusableStringBuilder).GetField("Get", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, new Func<IReusableStringBuilder>(Pool.Get));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Return(IReusableStringBuilder sb) => Pool.Return((ReusableStringBuilder) sb);
    }
}
