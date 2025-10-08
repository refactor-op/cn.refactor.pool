#nullable enable
using System.Collections.Generic;

namespace Refactor.Pool.Extra
{
    public static class QueuePoolPolicy
    {
        public readonly struct Default<T> : IPoolPolicy<Queue<T>>
        {
            public Queue<T> Create() => new();
            public void OnRent(Queue<T> obj) => obj.Clear();
            public bool OnReturn(Queue<T>? obj) => obj != null;
        }
    }
}