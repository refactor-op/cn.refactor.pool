using System.Collections.Generic;

namespace Refactor.Pool.Extra
{
    public static class QueuePool<T>
    {
        public static Pool<Queue<T>, QueuePoolPolicy.Default<T>> Default =
            Pools.Default<Queue<T>, QueuePoolPolicy.Default<T>>();
    }
}