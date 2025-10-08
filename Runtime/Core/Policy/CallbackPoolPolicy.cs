#nullable enable
using System;

namespace Refactor.Pool
{
    public static class CallbackPoolPolicy
    {
        public readonly struct Default<T> : IPoolPolicy<T> where T : class
        {
            private readonly Func<T>         _factory;
            private readonly Action<T>?      _onRent;
            private readonly Func<T?, bool>? _onReturn;

            public Default(Func<T> factory, Action<T>? onRent, Func<T?, bool>? onReturn)
            {
                _factory  = factory ?? throw new ArgumentNullException(nameof(factory));
                _onRent   = onRent;
                _onReturn = onReturn;
            }

            public T Create() => _factory();
            public void OnRent(T obj) => _onRent?.Invoke(obj);
            public bool OnReturn(T? obj) => obj != null && (_onReturn?.Invoke(obj) ?? true);
        }

        public readonly struct Default<T, TArg1> : IPoolPolicy<T, TArg1> where T : class
        {
            private readonly Func<TArg1, T>    _factory;
            private readonly Action<T, TArg1>? _onRent;
            private readonly Func<T?, bool>?   _onReturn;

            public Default(Func<TArg1, T> factory, Action<T, TArg1>? onRent, Func<T?, bool>? onReturn)
            {
                _factory  = factory ?? throw new ArgumentNullException(nameof(factory));
                _onRent   = onRent;
                _onReturn = onReturn;
            }

            public T Create(TArg1 arg1) => _factory(arg1);
            public void OnRent(T obj, TArg1 arg1) => _onRent?.Invoke(obj, arg1);
            public bool OnReturn(T? obj) => obj != null && (_onReturn?.Invoke(obj) ?? true);
        }

        public readonly struct Default<T, TArg1, TArg2> : IPoolPolicy<T, TArg1, TArg2> where T : class
        {
            private readonly Func<TArg1, TArg2, T>    _factory;
            private readonly Action<T, TArg1, TArg2>? _onRent;
            private readonly Func<T?, bool>?          _onReturn;

            public Default(
                Func<TArg1, TArg2, T> factory,
                Action<T, TArg1, TArg2>? onRent,
                Func<T?, bool>? onReturn)
            {
                _factory  = factory ?? throw new ArgumentNullException(nameof(factory));
                _onRent   = onRent;
                _onReturn = onReturn;
            }

            public T Create(TArg1 arg1, TArg2 arg2) => _factory(arg1, arg2);
            public void OnRent(T obj, TArg1 arg1, TArg2 arg2) => _onRent?.Invoke(obj, arg1, arg2);
            public bool OnReturn(T? obj) => obj != null && (_onReturn?.Invoke(obj) ?? true);
        }

        public readonly struct Default<T, TArg1, TArg2, TArg3> : IPoolPolicy<T, TArg1, TArg2, TArg3>
            where T : class
        {
            private readonly Func<TArg1, TArg2, TArg3, T>    _factory;
            private readonly Action<T, TArg1, TArg2, TArg3>? _onRent;
            private readonly Func<T?, bool>?                 _onReturn;

            public Default(
                Func<TArg1, TArg2, TArg3, T> factory,
                Action<T, TArg1, TArg2, TArg3>? onRent,
                Func<T?, bool>? onReturn)
            {
                _factory  = factory ?? throw new ArgumentNullException(nameof(factory));
                _onRent   = onRent;
                _onReturn = onReturn;
            }

            public T Create(TArg1 arg1, TArg2 arg2, TArg3 arg3) => _factory(arg1, arg2, arg3);
            public void OnRent(T obj, TArg1 arg1, TArg2 arg2, TArg3 arg3) => _onRent?.Invoke(obj, arg1, arg2, arg3);
            public bool OnReturn(T? obj) => obj != null && (_onReturn?.Invoke(obj) ?? true);
        }
    }
}