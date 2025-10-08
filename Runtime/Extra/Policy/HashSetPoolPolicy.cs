#nullable enable
using System.Collections.Generic;

namespace Refactor.Pool.Extra
{
    public static class HashSetPoolPolicy
    {
        public readonly struct Default<T> : IPoolPolicy<HashSet<T>>
        {
            public HashSet<T> Create() => new();
            public void OnRent(HashSet<T> obj) => obj.Clear();
            public bool OnReturn(HashSet<T>? obj) => obj != null;
        }
    }
}