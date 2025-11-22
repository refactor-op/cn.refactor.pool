namespace Refactor.Pooling
{
    public static partial class Pools
    {
        /// <summary>
        /// 创建池.
        /// </summary>
        public static Pool<T, TPolicy> Create<T, TPolicy>(TPolicy policy, int capacity)
            where T : class
            where TPolicy : struct, IPoolPolicy<T> =>
            new(policy, capacity);
    }
}