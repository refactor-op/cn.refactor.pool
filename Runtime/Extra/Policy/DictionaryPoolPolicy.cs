#nullable enable
using System.Collections.Generic;

namespace Refactor.Pool.Extra
{
    public static class DictionaryPoolPolicy
    {
        public readonly struct Default<TKey, TValue> : IPoolPolicy<Dictionary<TKey, TValue>> where TKey : notnull
        {
            public Dictionary<TKey, TValue> Create() => new();
            public void OnRent(Dictionary<TKey, TValue> obj) => obj.Clear();
            public bool OnReturn(Dictionary<TKey, TValue>? obj) => obj != null;
        }
    }
}