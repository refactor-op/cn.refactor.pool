using System.Collections.Generic;

namespace Refactor.Pool.Extra
{
    public static class HashSetPool<T>
    {
        public static Pool<HashSet<T>, HashSetPoolPolicy.Default<T>> Default =
            Pools.Default<HashSet<T>, HashSetPoolPolicy.Default<T>>();
    }
}