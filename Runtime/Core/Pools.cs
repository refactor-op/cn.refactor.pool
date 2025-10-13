#nullable enable

namespace Refactor.Core.Pool
{
    /// <summary>
    /// 对象池静态工厂.
    /// </summary>
    public static class Pools
    {
        /// <summary>
        /// 创建单线程对象池.
        /// </summary>
        public static Pool<T, TPolicy> Create<T, TPolicy>(
            TPolicy policy,
            int initialCapacity = 16,
            int maxCapacity = 64)
            where T : class
            where TPolicy : struct, IPoolPolicy<T>
            => new(policy, initialCapacity, maxCapacity);

        public static class Concurrent
        {
            /// <summary>
            /// 创建多线程安全对象池.
            /// </summary>
            public static ConcurrentPool<T, TPolicy> Create<T, TPolicy>(
                TPolicy policy,
                int localCapacity = 16,
                int sharedCapacity = 64)
                where T : class
                where TPolicy : struct, IPoolPolicy<T>
                => new(policy, localCapacity, sharedCapacity);
        }
    }
}