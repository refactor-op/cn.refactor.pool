namespace Refactor.Pooling
{
    /// <para>
    /// 实现此接口的实体必须是 struct 或 readonly struct.
    /// </para>
    /// <para>
    /// 为防止 <c>Pool</c> 调用接口方法时发生拷贝从而导致性能下降,
    /// 所有实现此接口的方法在 <c>struct</c> 中都必须被声明为 <c>readonly</c>.
    /// </para>
    /// <para>
    /// <b>示例:</b>
    /// <code>
    /// public struct MyPolicy : IPoolPolicy&lt;MyObject&gt;
    /// {
    ///     // 错误: 方法没有标记 readonly.
    ///     public MyObject Create() { /* ... */ }
    ///
    ///     // 正确: 方法标记 readonly.
    ///     public readonly void OnRent(MyObject obj) { /* ... */ }
    ///
    ///     // 正确: 方法标记 readonly.
    ///     public readonly bool OnReturn(MyObject obj) { /* ... */ }
    /// }
    /// </code>
    /// </para>
    public interface IPoolPolicy<T> where T : class
    {
        /// <summary>
        /// 创建新对象 (池为空时调用).
        /// </summary>
        T Create();

        /// <summary>
        /// 每次 Rent 时调用.
        /// 重置.
        /// </summary>
        void OnRent(T obj);

        /// <summary>
        /// 每次 Return 时调用.
        /// 清理.
        /// </summary>
        /// <returns>true 表示接受对象, false 表示丢弃对象.</returns>
        bool OnReturn(T obj);
    }

    /// <summary> 离开 using 作用域时, 会自动回收对象. </summary>
    /// <summary>
    /// 池化对象的 RAII 句柄（ref struct，零分配）
    /// </summary>
    public ref struct PooledScope<T, TPolicy>
        where T : class
        where TPolicy : struct, IPoolPolicy<T>
    {
        private Pool<T, TPolicy> _pool;
        private T _value;
        
        internal PooledScope(Pool<T, TPolicy> pool, T value)
        {
            _pool  = pool;
            _value = value;
        }
        
        /// <summary>
        /// 获取池化对象
        /// </summary>
        public T Value => _value;
        
        /// <summary>
        /// 归还对象到池
        /// </summary>
        public void Dispose()
        {
            if (_value != null && _pool != null)
            {
                _pool.Return(_value);
                _value = null;
            }
        }
    }
}