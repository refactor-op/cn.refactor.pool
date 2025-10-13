#nullable enable

using System;
using System.Collections.Generic;

namespace Refactor.Core.Pool
{
    /// <summary>
    /// 单线程对象池.
    /// </summary>
    public sealed class Pool<T, TPolicy> : IPool<T>, IDisposable
        where T : class
        where TPolicy : struct, IPoolPolicy<T>
    {
        private readonly Stack<T> _stack;
        private readonly TPolicy  _policy;
        private readonly int      _max;

        public Pool(TPolicy policy, int initialCapacity = 16, int maxCapacity = 64)
        {
            if (initialCapacity <= 0 || maxCapacity <= 0)
                throw new ArgumentOutOfRangeException();

            _stack  = new Stack<T>(initialCapacity);
            _policy = policy;
            _max    = maxCapacity;
        }

        public T Rent()
        {
            var obj = _stack.Count > 0 ? _stack.Pop() : _policy.Create();
            _policy.OnRent(obj);
            return obj;
        }

        public void Return(T? obj)
        {
            if (obj == null) return;
            if (_stack.Count >= _max) return;
            if (_policy.OnReturn(obj)) _stack.Push(obj);
        }

        public PooledObject<T> RentScoped() => new(this, Rent());

        public void Prewarm(int count)
        {
            if (count <= 0) return;
            for (var i = 0; i < count && _stack.Count < _max; i++)
                _stack.Push(_policy.Create());
        }

        public void Clear() => _stack.Clear();

        public void Dispose() => Clear();
    }
}