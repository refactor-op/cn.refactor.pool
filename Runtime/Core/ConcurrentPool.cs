#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Refactor.Pool
{
    /// <summary>
    /// 多线程对象池.
    /// </summary>
    public sealed class ConcurrentPool<T, TPolicy> : IPool<T>, IDisposable
        where T : class
        where TPolicy : struct, IPoolPolicy<T>
    {
        private readonly ThreadLocal<Stack<T>> _localCache;
        private readonly ConcurrentStack<T> _shared;
        private readonly TPolicy _policy;
        private readonly int _localCapacity;
        private readonly int _sharedCapacity;

        public ConcurrentPool(
            TPolicy policy,
            int localCapacity = 16,
            int sharedCapacity = 64)
        {
            if (localCapacity <= 0 || sharedCapacity <= 0)
                throw new ArgumentOutOfRangeException();

            _policy = policy;
            _localCapacity = localCapacity;
            _sharedCapacity = sharedCapacity;
            _shared = new ConcurrentStack<T>();
            _localCache = new ThreadLocal<Stack<T>>(
                () => new Stack<T>(_localCapacity),
                trackAllValues: false
            );
        }

        public T Rent()
        {
            // 快速路径: 线程本地缓存.
            var local = _localCache.Value!;
            if (local.Count > 0)
            {
                var obj = local.Pop();
                _policy.OnRent(obj);
                return obj;
            }

            // 中速路径: 共享池.
            if (_shared.TryPop(out var obj2))
            {
                _policy.OnRent(obj2);
                return obj2;
            }

            // 慢速路径: 创建新对象.
            var newObj = _policy.Create();
            _policy.OnRent(newObj);
            return newObj;
        }

        public void Return(T? obj)
        {
            if (obj == null) return;
            if (!_policy.OnReturn(obj)) return;

            // 快速路径: 返回到线程本地缓存.
            var local = _localCache.Value!;
            if (local.Count < _localCapacity)
            {
                local.Push(obj);
                return;
            }

            // 慢速路径: 返回到共享池.
            if (_shared.Count < _sharedCapacity)
                _shared.Push(obj);
            
            // 池已满, 丢弃对象.
        }

        public PooledObject<T> RentScoped() => new(this, Rent());

        public void Prewarm(int count)
        {
            if (count <= 0) return;
            for (var i = 0; i < count && _shared.Count < _sharedCapacity; i++)
                _shared.Push(_policy.Create());
        }

        public void Clear()
        {
            while (_shared.TryPop(out _)) { }
            _localCache.Value?.Clear();
        }

        public void Dispose()
        {
            Clear();
            _localCache.Dispose();
        }
    }
}