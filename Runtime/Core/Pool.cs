#nullable enable

using System;
using System.Collections.Generic;

namespace Refactor.Pool
{
    public sealed class Pool<T, TPolicy> : IPool<T>, IDisposable
        where T : class
        where TPolicy : struct, IPoolPolicy<T>
    {
        private readonly Stack<T> _stack;
        private readonly TPolicy  _policy;
        private readonly int      _max;

        public Pool(TPolicy policy, int init = 16, int max = 64)
        {
            if (init <= 0 || max <= 0)
                throw new ArgumentOutOfRangeException(nameof(init));

            _stack  = new Stack<T>(init);
            _policy = policy;
            _max    = max;
        }

        public void Dispose()
        {
            Clear();
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

        public void Clear()
        {
            _stack.Clear();
        }
    }

    public sealed class Pool<T, TArg1, TPolicy> : IPool<T>, IDisposable
        where T : class
        where TPolicy : struct, IPoolPolicy<T, TArg1>
    {
        private readonly Stack<T> _stack;
        private readonly TPolicy  _policy;
        private readonly int      _max;
        private readonly TArg1    _arg1;

        public Pool(TPolicy policy, TArg1 arg1, int init = 16, int max = 64)
        {
            if (init <= 0 || max <= 0)
                throw new ArgumentOutOfRangeException(nameof(init));

            _policy = policy;
            _arg1   = arg1;
            _stack  = new Stack<T>(init);
            _max    = max;
        }

        public void Dispose()
        {
            Clear();
        }

        public T Rent()
        {
            var obj = _stack.Count > 0 ? _stack.Pop() : _policy.Create(_arg1);
            _policy.OnRent(obj, _arg1);
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
                _stack.Push(_policy.Create(_arg1));
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }

    public sealed class Pool<T, TArg1, TArg2, TPolicy> : IPool<T>, IDisposable
        where T : class
        where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2>
    {
        private readonly Stack<T> _stack;
        private readonly TPolicy  _policy;
        private readonly int      _max;
        private readonly TArg1    _arg1;
        private readonly TArg2    _arg2;

        public Pool(TPolicy policy, TArg1 arg1, TArg2 arg2, int init = 16, int max = 64)
        {
            if (init <= 0 || max <= 0)
                throw new ArgumentOutOfRangeException(nameof(init));

            _policy = policy;
            _arg1   = arg1;
            _arg2   = arg2;
            _stack  = new Stack<T>(init);
            _max    = max;
        }

        public void Dispose()
        {
            Clear();
        }

        public T Rent()
        {
            var obj = _stack.Count > 0 ? _stack.Pop() : _policy.Create(_arg1, _arg2);
            _policy.OnRent(obj, _arg1, _arg2);
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
                _stack.Push(_policy.Create(_arg1, _arg2));
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }

    public sealed class Pool<T, TArg1, TArg2, TArg3, TPolicy> : IPool<T>, IDisposable
        where T : class
        where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2, TArg3>
    {
        private readonly Stack<T> _stack;
        private readonly TPolicy  _policy;
        private readonly int      _max;
        private readonly TArg1    _arg1;
        private readonly TArg2    _arg2;
        private readonly TArg3    _arg3;

        public Pool(
            TPolicy policy,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            int init = 16,
            int max = 64)
        {
            if (init <= 0 || max <= 0)
                throw new ArgumentOutOfRangeException(nameof(init));

            _policy = policy;
            _arg1   = arg1;
            _arg2   = arg2;
            _arg3   = arg3;
            _stack  = new Stack<T>(init);
            _max    = max;
        }

        public void Dispose()
        {
            Clear();
        }

        public T Rent()
        {
            var obj = _stack.Count > 0 ? _stack.Pop() : _policy.Create(_arg1, _arg2, _arg3);
            _policy.OnRent(obj, _arg1, _arg2, _arg3);
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
                _stack.Push(_policy.Create(_arg1, _arg2, _arg3));
        }

        public void Clear()
        {
            _stack.Clear();
        }
    }
}