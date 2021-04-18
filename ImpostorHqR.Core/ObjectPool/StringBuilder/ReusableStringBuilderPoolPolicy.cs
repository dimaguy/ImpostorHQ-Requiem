using Microsoft.Extensions.ObjectPool;

namespace ImpostorHqR.Core.ObjectPool.StringBuilder
{
    public class ReusableStringBuilderPoolPolicy : IPooledObjectPolicy<ReusableStringBuilder>
    {
        /// <summary>
        /// If this is surpassed, the cost of clearing the builder surpasses the cost of getting a new one.
        /// </summary>
        private const int MaxCapacity = 4096;

        public ReusableStringBuilder Create() => new ReusableStringBuilder(new System.Text.StringBuilder());

        public bool Return(ReusableStringBuilder obj)
        {
            if (obj.StringBuilder.Capacity > MaxCapacity) return false;
            obj.Clear();
            return true;
        }
    }
}
