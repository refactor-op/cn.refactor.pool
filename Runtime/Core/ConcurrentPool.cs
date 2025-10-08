#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Refactor.Pool
{
    public sealed class ConcurrentPool<T, TPolicy> : IPool<T>, IDisposable
        where T : class
        where TPolicy : struct, IPoolPolicy<T>
    {
        // 三层架构设计:
        // 1. ThreadLocal 线程局部缓存 (快速路径).
        // 2. ConcurrentStack 共享池 (中速路径).
        // 3. Policy.Create() 创建新对象 (慢速路径).
        private readonly ThreadLocal<Stack<T>> _localCache;
        private readonly ConcurrentStack<T>    _shared;
        private readonly TPolicy               _policy;
        private readonly int                   _localCacheCapacity;
        private readonly int                   _sharedCapacity;

        public ConcurrentPool(TPolicy policy, int localCacheCapacity = 16, int sharedCapacity = 64)
        {
            if (localCacheCapacity <= 0 || sharedCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(localCacheCapacity));

            _policy             = policy;
            _localCacheCapacity = localCacheCapacity;
            _sharedCapacity     = sharedCapacity;
            _shared             = new ConcurrentStack<T>();

            // 使用 valueFactory 避免捕获 this,
            // 捕获字段而非参数, 避免闭包问题.
            _localCache = new ThreadLocal<Stack<T>>(
                () => new Stack<T>(_localCacheCapacity),
                false
            );
        }

        public void Dispose()
        {
            Clear();
            _localCache.Dispose();
        }

        public T Rent()
        {
            // 快速路径: 从线程局部缓存获取.
            var local = _localCache.Value!;
            if (local.Count > 0)
            {
                var obj = local.Pop();
                _policy.OnRent(obj);
                return obj;
            }

            // 中速路径: 从共享池获取.
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

            // 快速路径: 返回到线程局部缓存.
            var local = _localCache.Value!;
            if (local.Count < _localCacheCapacity)
            {
                local.Push(obj);
                return;
            }

            // 慢速路径: 返回到共享池.
            if (_shared.Count < _sharedCapacity) _shared.Push(obj);

            // 池已满, 丢弃对象（让 Gc 回收）.
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
            while (_shared.TryPop(out _))
            {
            }

            _localCache.Value?.Clear();
        }
    }

    public sealed class ConcurrentPool<T, TArg1, TPolicy> : IPool<T>, IDisposable
        where T : class
        where TPolicy : struct, IPoolPolicy<T, TArg1>
    {
        private readonly ThreadLocal<Stack<T>> _localCache;
        private readonly ConcurrentStack<T>    _shared;
        private readonly TPolicy               _policy;
        private readonly int                   _localCacheCapacity;
        private readonly int                   _sharedCapacity;
        private readonly TArg1                 _arg1;

        public ConcurrentPool(
            TPolicy policy,
            TArg1 arg1,
            int localCacheCapacity = 16,
            int sharedCapacity = 64)
        {
            if (localCacheCapacity <= 0 || sharedCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(localCacheCapacity));
            _policy             = policy;
            _arg1               = arg1;
            _localCacheCapacity = localCacheCapacity;
            _sharedCapacity     = sharedCapacity;
            _shared             = new ConcurrentStack<T>();
            _localCache = new ThreadLocal<Stack<T>>(
                () => new Stack<T>(_localCacheCapacity),
                false
            );
        }

        public void Dispose()
        {
            Clear();
            _localCache.Dispose();
        }

        public T Rent()
        {
            var local = _localCache.Value!;
            if (local.Count > 0)
            {
                var obj = local.Pop();
                _policy.OnRent(obj, _arg1);
                return obj;
            }

            if (_shared.TryPop(out var obj2))
            {
                _policy.OnRent(obj2, _arg1);
                return obj2;
            }

            var newObj = _policy.Create(_arg1);
            _policy.OnRent(newObj, _arg1);
            return newObj;
        }

        public void Return(T? obj)
        {
            if (obj == null) return;
            if (!_policy.OnReturn(obj)) return;

            var local = _localCache.Value!;
            if (local.Count < _localCacheCapacity)
            {
                local.Push(obj);
                return;
            }

            if (_shared.Count < _sharedCapacity)
                _shared.Push(obj);
        }

        public PooledObject<T> RentScoped() => new(this, Rent());

        public void Prewarm(int count)
        {
            if (count <= 0) return;
            for (var i = 0; i < count && _shared.Count < _sharedCapacity; i++)
                _shared.Push(_policy.Create(_arg1));
        }

        public void Clear()
        {
            while (_shared.TryPop(out _))
            {
            }

            _localCache.Value?.Clear();
        }
    }

    public sealed class ConcurrentPool<T, TArg1, TArg2, TPolicy> : IPool<T>, IDisposable
        where T : class
        where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2>
    {
        private readonly ThreadLocal<Stack<T>> _localCache;
        private readonly ConcurrentStack<T>    _shared;
        private readonly TPolicy               _policy;
        private readonly int                   _localCacheCapacity;
        private readonly int                   _sharedCapacity;
        private readonly TArg1                 _arg1;
        private readonly TArg2                 _arg2;

        public ConcurrentPool(
            TPolicy policy,
            TArg1 arg1,
            TArg2 arg2,
            int localCacheCapacity = 16,
            int sharedCapacity = 64)
        {
            if (localCacheCapacity <= 0 || sharedCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(localCacheCapacity));
            _policy             = policy;
            _arg1               = arg1;
            _arg2               = arg2;
            _localCacheCapacity = localCacheCapacity;
            _sharedCapacity     = sharedCapacity;
            _shared             = new ConcurrentStack<T>();
            _localCache = new ThreadLocal<Stack<T>>(
                () => new Stack<T>(_localCacheCapacity),
                false
            );
        }

        public void Dispose()
        {
            Clear();
            _localCache.Dispose();
        }

        public T Rent()
        {
            var local = _localCache.Value!;
            if (local.Count > 0)
            {
                var obj = local.Pop();
                _policy.OnRent(obj, _arg1, _arg2);
                return obj;
            }

            if (_shared.TryPop(out var obj2))
            {
                _policy.OnRent(obj2, _arg1, _arg2);
                return obj2;
            }

            var newObj = _policy.Create(_arg1, _arg2);
            _policy.OnRent(newObj, _arg1, _arg2);
            return newObj;
        }

        public void Return(T? obj)
        {
            if (obj == null) return;
            if (!_policy.OnReturn(obj)) return;

            var local = _localCache.Value!;
            if (local.Count < _localCacheCapacity)
            {
                local.Push(obj);
                return;
            }

            if (_shared.Count < _sharedCapacity)
                _shared.Push(obj);
        }

        public PooledObject<T> RentScoped() => new(this, Rent());

        public void Prewarm(int count)
        {
            if (count <= 0) return;
            for (var i = 0; i < count && _shared.Count < _sharedCapacity; i++)
                _shared.Push(_policy.Create(_arg1, _arg2));
        }

        public void Clear()
        {
            while (_shared.TryPop(out _))
            {
            }

            _localCache.Value?.Clear();
        }
    }

    public sealed class ConcurrentPool<T, TArg1, TArg2, TArg3, TPolicy> : IPool<T>, IDisposable
        where T : class
        where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2, TArg3>
    {
        private readonly ThreadLocal<Stack<T>> _localCache;
        private readonly ConcurrentStack<T>    _shared;
        private readonly TPolicy               _policy;
        private readonly int                   _localCacheCapacity;
        private readonly int                   _sharedCapacity;
        private readonly TArg1                 _arg1;
        private readonly TArg2                 _arg2;
        private readonly TArg3                 _arg3;

        public ConcurrentPool(
            TPolicy policy,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            int localCacheCapacity = 16,
            int sharedCapacity = 64)
        {
            if (localCacheCapacity <= 0 || sharedCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(localCacheCapacity));
            _policy             = policy;
            _arg1               = arg1;
            _arg2               = arg2;
            _arg3               = arg3;
            _localCacheCapacity = localCacheCapacity;
            _sharedCapacity     = sharedCapacity;
            _shared             = new ConcurrentStack<T>();
            _localCache = new ThreadLocal<Stack<T>>(
                () => new Stack<T>(_localCacheCapacity),
                false
            );
        }

        public void Dispose()
        {
            Clear();
            _localCache.Dispose();
        }

        public T Rent()
        {
            var local = _localCache.Value!;
            if (local.Count > 0)
            {
                var obj = local.Pop();
                _policy.OnRent(obj, _arg1, _arg2, _arg3);
                return obj;
            }

            if (_shared.TryPop(out var obj2))
            {
                _policy.OnRent(obj2, _arg1, _arg2, _arg3);
                return obj2;
            }

            var newObj = _policy.Create(_arg1, _arg2, _arg3);
            _policy.OnRent(newObj, _arg1, _arg2, _arg3);
            return newObj;
        }

        public void Return(T? obj)
        {
            if (obj == null) return;
            if (!_policy.OnReturn(obj)) return;

            var local = _localCache.Value!;
            if (local.Count < _localCacheCapacity)
            {
                local.Push(obj);
                return;
            }

            if (_shared.Count < _sharedCapacity)
                _shared.Push(obj);
        }

        public PooledObject<T> RentScoped() => new(this, Rent());

        public void Prewarm(int count)
        {
            if (count <= 0) return;
            for (var i = 0; i < count && _shared.Count < _sharedCapacity; i++)
                _shared.Push(_policy.Create(_arg1, _arg2, _arg3));
        }

        public void Clear()
        {
            while (_shared.TryPop(out _))
            {
            }

            _localCache.Value?.Clear();
        }
    }
}