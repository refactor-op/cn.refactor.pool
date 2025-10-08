#nullable enable
using System.Collections.Generic;

namespace Refactor.Pool.Extra
{
    public static class ListPoolPolicy
    {
        public readonly struct Default<T> : IPoolPolicy<List<T>>
        {
            public List<T> Create() => new();
            public void OnRent(List<T> obj) => obj.Clear();
            public bool OnReturn(List<T>? obj) => obj != null;
        }
    }
}