using System.Collections.Generic;

namespace Refactor.Pool.Extra
{
    public static class DictionaryPool<TKey, TValue>
    {
        public static Pool<Dictionary<TKey, TValue>, DictionaryPoolPolicy.Default<TKey, TValue>> Default =
            Pools.Default<Dictionary<TKey, TValue>, DictionaryPoolPolicy.Default<TKey, TValue>>();
    }
}