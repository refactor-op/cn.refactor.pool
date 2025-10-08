#nullable enable
using System;

namespace Refactor.Pool
{
    public static class Pools
    {
        public static Pool<T, TPolicy> Create<T, TPolicy>(TPolicy policy, int initialCapacity = 16, int max = 64)
            where T : class where TPolicy : struct, IPoolPolicy<T> => new(policy, initialCapacity, max);

        public static Pool<T, TArg1, TPolicy> Create<T, TArg1, TPolicy>(
            TPolicy policy,
            TArg1 arg1,
            int initialCapacity = 16,
            int max = 64)
            where T : class where TPolicy : struct, IPoolPolicy<T, TArg1> => new(policy, arg1, initialCapacity, max);

        public static Pool<T, TArg1, TArg2, TPolicy> Create<T, TArg1, TArg2, TPolicy>(
            TPolicy policy,
            TArg1 arg1,
            TArg2 arg2,
            int initialCapacity = 16,
            int max = 64)
            where T : class where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2> =>
            new(policy, arg1, arg2, initialCapacity, max);

        public static Pool<T, TArg1, TArg2, TArg3, TPolicy> Create<T, TArg1, TArg2, TArg3, TPolicy>(
            TPolicy policy,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            int initialCapacity = 16,
            int max = 64)
            where T : class where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2, TArg3> =>
            new(policy, arg1, arg2, arg3, initialCapacity, max);

        public static Pool<T, TPolicy> Default<T, TPolicy>(int initialCapacity = 16, int max = 64)
            where T : class where TPolicy : struct, IPoolPolicy<T> => new(default, initialCapacity, max);


        public static Pool<T, TArg1, TPolicy> Default<T, TArg1, TPolicy>(
            TArg1 arg1,
            int initialCapacity = 16,
            int max = 64)
            where T : class where TPolicy : struct, IPoolPolicy<T, TArg1> => new(default, arg1, initialCapacity, max);

        public static Pool<T, TArg1, TArg2, TPolicy> Default<T, TArg1, TArg2, TPolicy>(
            TArg1 arg1,
            TArg2 arg2,
            int initialCapacity = 16,
            int max = 64)
            where T : class where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2> =>
            new(default, arg1, arg2, initialCapacity, max);

        public static Pool<T, TArg1, TArg2, TArg3, TPolicy> Default<T, TArg1, TArg2, TArg3, TPolicy>(
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            int initialCapacity = 16,
            int max = 64)
            where T : class where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2, TArg3> =>
            new(default, arg1, arg2, arg3, initialCapacity, max);

        public static Pool<T, CallbackPoolPolicy.Default<T>> FromCallback<T>(
            Func<T> factory,
            Action<T>? onRent = null,
            Func<T?, bool>? onReturn = null,
            int initialCapacity = 16,
            int max = 64)
            where T : class =>
            new(new CallbackPoolPolicy.Default<T>(factory, onRent, onReturn), initialCapacity, max);

        public static Pool<T, TArg1, CallbackPoolPolicy.Default<T, TArg1>> FromCallback<T, TArg1>(
            Func<TArg1, T> factory,
            TArg1 arg1,
            Action<T, TArg1>? onRent = null,
            Func<T?, bool>? onReturn = null,
            int initialCapacity = 16,
            int max = 64)
            where T : class =>
            new(new CallbackPoolPolicy.Default<T, TArg1>(factory, onRent, onReturn), arg1, initialCapacity, max);

        public static Pool<T, TArg1, TArg2, CallbackPoolPolicy.Default<T, TArg1, TArg2>> FromCallback<T, TArg1, TArg2>(
            Func<TArg1, TArg2, T> factory,
            TArg1 arg1,
            TArg2 arg2,
            Action<T, TArg1, TArg2>? onRent = null,
            Func<T?, bool>? onReturn = null,
            int initialCapacity = 16,
            int max = 64)
            where T : class =>
            new(new CallbackPoolPolicy.Default<T, TArg1, TArg2>(factory, onRent, onReturn),
                arg1,
                arg2,
                initialCapacity,
                max);

        public static Pool<T, TArg1, TArg2, TArg3, CallbackPoolPolicy.Default<T, TArg1, TArg2, TArg3>> FromCallback<T,
            TArg1, TArg2, TArg3>(
            Func<TArg1, TArg2, TArg3, T> factory,
            TArg1 arg1,
            TArg2 arg2,
            TArg3 arg3,
            Action<T, TArg1, TArg2, TArg3>? onRent = null,
            Func<T?, bool>? onReturn = null,
            int initialCapacity = 16,
            int max = 64)
            where T : class =>
            new(
                new CallbackPoolPolicy.Default<T, TArg1, TArg2, TArg3>(factory, onRent, onReturn),
                arg1,
                arg2,
                arg3,
                initialCapacity,
                max);

        public static class Concurrent
        {
            public static ConcurrentPool<T, TPolicy> Create<T, TPolicy>(
                TPolicy policy,
                int initialCapacity = 16,
                int max = 64)
                where T : class where TPolicy : struct, IPoolPolicy<T> => new(policy, initialCapacity, max);

            public static ConcurrentPool<T, TArg1, TPolicy> Create<T, TArg1, TPolicy>(
                TPolicy policy,
                TArg1 arg1,
                int initialCapacity = 16,
                int max = 64)
                where T : class where TPolicy : struct, IPoolPolicy<T, TArg1> =>
                new(policy, arg1, initialCapacity, max);

            public static ConcurrentPool<T, TArg1, TArg2, TPolicy> Create<T, TArg1, TArg2, TPolicy>(
                TPolicy policy,
                TArg1 arg1,
                TArg2 arg2,
                int initialCapacity = 16,
                int max = 64)
                where T : class where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2> =>
                new(policy, arg1, arg2, initialCapacity, max);

            public static ConcurrentPool<T, TArg1, TArg2, TArg3, TPolicy> Create<T, TArg1, TArg2, TArg3, TPolicy>(
                TPolicy policy,
                TArg1 arg1,
                TArg2 arg2,
                TArg3 arg3,
                int initialCapacity = 16,
                int max = 64)
                where T : class where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2, TArg3> =>
                new(policy, arg1, arg2, arg3, initialCapacity, max);

            public static ConcurrentPool<T, TPolicy> Default<T, TPolicy>(int initialCapacity = 16, int max = 64)
                where T : class where TPolicy : struct, IPoolPolicy<T> => new(default, initialCapacity, max);


            public static ConcurrentPool<T, TArg1, TPolicy> Default<T, TArg1, TPolicy>(
                TArg1 arg1,
                int initialCapacity = 16,
                int max = 64)
                where T : class where TPolicy : struct, IPoolPolicy<T, TArg1> =>
                new(default, arg1, initialCapacity, max);

            public static ConcurrentPool<T, TArg1, TArg2, TPolicy> Default<T, TArg1, TArg2, TPolicy>(
                TArg1 arg1,
                TArg2 arg2,
                int initialCapacity = 16,
                int max = 64)
                where T : class where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2> =>
                new(default, arg1, arg2, initialCapacity, max);

            public static ConcurrentPool<T, TArg1, TArg2, TArg3, TPolicy> Default<T, TArg1, TArg2, TArg3, TPolicy>(
                TArg1 arg1,
                TArg2 arg2,
                TArg3 arg3,
                int initialCapacity = 16,
                int max = 64)
                where T : class where TPolicy : struct, IPoolPolicy<T, TArg1, TArg2, TArg3> =>
                new(default, arg1, arg2, arg3, initialCapacity, max);

            public static ConcurrentPool<T, CallbackPoolPolicy.Default<T>> FromCallback<T>(
                Func<T> factory,
                Action<T>? onRent = null,
                Func<T?, bool>? onReturn = null,
                int initialCapacity = 16,
                int max = 64)
                where T : class =>
                new(new CallbackPoolPolicy.Default<T>(factory, onRent, onReturn), initialCapacity, max);

            public static ConcurrentPool<T, TArg1, CallbackPoolPolicy.Default<T, TArg1>> FromCallback<T, TArg1>(
                Func<TArg1, T> factory,
                TArg1 arg1,
                Action<T, TArg1>? onRent = null,
                Func<T?, bool>? onReturn = null,
                int initialCapacity = 16,
                int max = 64)
                where T : class =>
                new(new CallbackPoolPolicy.Default<T, TArg1>(factory, onRent, onReturn), arg1, initialCapacity, max);

            public static ConcurrentPool<T, TArg1, TArg2, CallbackPoolPolicy.Default<T, TArg1, TArg2>> FromCallback<T,
                TArg1,
                TArg2>(
                Func<TArg1, TArg2, T> factory,
                TArg1 arg1,
                TArg2 arg2,
                Action<T, TArg1, TArg2>? onRent = null,
                Func<T?, bool>? onReturn = null,
                int initialCapacity = 16,
                int max = 64)
                where T : class =>
                new(new CallbackPoolPolicy.Default<T, TArg1, TArg2>(factory, onRent, onReturn),
                    arg1,
                    arg2,
                    initialCapacity,
                    max);

            public static ConcurrentPool<T, TArg1, TArg2, TArg3, CallbackPoolPolicy.Default<T, TArg1, TArg2, TArg3>>
                FromCallback<
                    T,
                    TArg1, TArg2, TArg3>(
                    Func<TArg1, TArg2, TArg3, T> factory,
                    TArg1 arg1,
                    TArg2 arg2,
                    TArg3 arg3,
                    Action<T, TArg1, TArg2, TArg3>? onRent = null,
                    Func<T?, bool>? onReturn = null,
                    int initialCapacity = 16,
                    int max = 64)
                where T : class =>
                new(
                    new CallbackPoolPolicy.Default<T, TArg1, TArg2, TArg3>(factory, onRent, onReturn),
                    arg1,
                    arg2,
                    arg3,
                    initialCapacity,
                    max);
        }
    }
}