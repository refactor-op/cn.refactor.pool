#nullable enable

using System;

namespace Refactor.Pool
{
    /// <summary>
    /// 对象池策略接口.
    /// 实现必须是 readonly struct.
    /// </summary>
    public interface IPoolPolicy<T> where T : class
    {
        /// <summary>
        /// 创建新对象 (池为空时调用).
        /// </summary>
        T Create();
        
        /// <summary>
        /// 每次 Rent 时调用.
        /// 最佳实践是将清理逻辑放在这里.
        /// </summary>
        void OnRent(T obj);
        
        /// <summary>
        /// 每次 Return 时调用.
        /// 最佳实践是将能否放入池中的判断逻辑放在这里, 而不是清理逻辑.
        /// </summary>
        /// <returns>true 表示接受对象, false 表示丢弃对象.</returns>
        bool OnReturn(T? obj);
    }

    public interface IPool<T> where T : class
    {
        T Rent();
        void Return(T? obj);
    }

    /// <summary>
    /// RAII 风格的池化对象句柄.
    /// </summary>
    public ref struct PooledObject<T> where T : class
    {
        private IPool<T>? _pool;
        private T?        _value;

        internal PooledObject(IPool<T> pool, T value)
        {
            _pool  = pool;
            _value = value;
        }

        public T Value => _value ?? throw new ObjectDisposedException(nameof(PooledObject<T>));

        public static implicit operator T(PooledObject<T> pooled) => pooled.Value;

        public void Dispose()
        {
            if (_value != null && _pool != null)
            {
                _pool.Return(_value);
                _value = null;
                _pool  = null;
            }
        }
    }
}