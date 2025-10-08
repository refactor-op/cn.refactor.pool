using System.Collections.Generic;

namespace Refactor.Pool.Extra
{
    public static class StackPool<T>
    {
        public static Pool<Stack<T>, StackPoolPolicy.Default<T>> Default =
            Pools.Default<Stack<T>, StackPoolPolicy.Default<T>>();
    }
}