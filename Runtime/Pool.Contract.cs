#nullable enable

using System;

namespace Refactor.Pool
{
    #region Policy

    /// <summary>
    /// 实现该接口的实体必须是只读的结构体 (readonly struct).
    /// </summary>
    public interface IPoolPolicy<T> where T : class
    {
        /// <summary>
        /// 创建新对象实例 (仅在池为空或预热时调用).
        /// </summary>
        T Create();
        
        /// <summary>
        /// 准备对象供租用 (每次 Rent 都会调用, 无论新建还是重用)
        /// </summary>
        void OnRent(T obj);
        
        /// <summary>
        /// 归还对象前的验证和清理 (每次 Return 时调用).
        /// </summary>
        bool OnReturn(T? obj);
    }

    /// <summary>
    /// 实现该接口的实体必须是只读的结构体 (readonly struct).
    /// </summary>
    public interface IPoolPolicy<T, in TArg1> where T : class
    {
        T Create(TArg1 arg1);
        void OnRent(T obj, TArg1 arg1);
        bool OnReturn(T? obj);
    }

    /// <summary>
    /// 实现该接口的实体必须是只读的结构体 (readonly struct).
    /// </summary>
    public interface IPoolPolicy<T, in TArg1, in TArg2> where T : class
    {
        T Create(TArg1 arg1, TArg2 arg2);
        void OnRent(T obj, TArg1 arg1, TArg2 arg2);
        bool OnReturn(T? obj);
    }

    /// <summary>
    /// 实现该接口的实体必须是只读的结构体 (readonly struct).
    /// </summary>
    public interface IPoolPolicy<T, in TArg1, in TArg2, in TArg3> where T : class
    {
        T Create(TArg1 arg1, TArg2 arg2, TArg3 arg3);
        void OnRent(T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3);
        bool OnReturn(T? obj);
    }

    #endregion

    #region Pool

    public interface IPool<T> where T : class
    {
        T Rent();
        void Return(T? obj);
    }

    #endregion

    #region Scoped Handle

    public ref struct PooledObject<T> where T : class
    {
        private IPool<T>? _pool;
        private T?        _value;  // 可空, 表示已 Dispose.

        internal PooledObject(IPool<T> pool, T value)
        {
            _pool  = pool;
            _value = value;
        }

        public static implicit operator T(PooledObject<T> pooled) => pooled.Value;

        public T Value => _value ?? throw new ObjectDisposedException(nameof(PooledObject<T>));

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

    #endregion
}