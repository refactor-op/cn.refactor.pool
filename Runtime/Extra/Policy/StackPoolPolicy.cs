#nullable enable
using System.Collections.Generic;

namespace Refactor.Pool.Extra
{
    public static class StackPoolPolicy
    {
        public readonly struct Default<T> : IPoolPolicy<Stack<T>>
        {
            public Stack<T> Create() => new();
            public void OnRent(Stack<T> obj) => obj.Clear();
            public bool OnReturn(Stack<T>? obj) => obj != null;
        }
    }
}