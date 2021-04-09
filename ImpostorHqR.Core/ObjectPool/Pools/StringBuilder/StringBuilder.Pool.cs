using ImpostorHqR.Extension.Api.Interface.Helpers.ObjectPool;
using ImpostorHqR.Core.Logging;
using ImpostorHqR.Extension.Api.Interface.Helpers.ObjectPool.Included;
using Microsoft.Extensions.ObjectPool;

namespace ImpostorHqR.Core.ObjectPool.Pools.StringBuilder
{
    public class StringBuilderPool : IReusableStringBuilderPool
    {
        public static readonly StringBuilderPool Instance = new StringBuilderPool();

        public static readonly ObjectPool<System.Text.StringBuilder> Pool = new DefaultObjectPool<System.Text.StringBuilder>(new StringBuilderPooledObjectPolicy());

        public IReusableStringBuilder Get()
        {
            return new ReusableStringBuilder(Pool.Get());
        }

        public void Return(IReusableStringBuilder sb)
        {
            if (sb.Used)
            {
                ConsoleLogging.Instance.LogError("String Builder Pool: Attempted to return a string builder that was already returned before.", this, true);
                return;
            }

            sb.Dispose();
        }
    }
}
