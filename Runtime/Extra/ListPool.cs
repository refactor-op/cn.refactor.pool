using System.Collections.Generic;

namespace Refactor.Pool.Extra
{
    public static class ListPool<T>
    {
        public static Pool<List<T>, ListPoolPolicy.Default<T>> Default =
            Pools.Default<List<T>, ListPoolPolicy.Default<T>>();
    }
}