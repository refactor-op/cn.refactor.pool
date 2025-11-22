using System.Collections.Generic;

namespace Refactor.Pooling
{
    /// <summary>
    /// 全局共享 List 池.
    /// </summary>
    public static class ListPool<T>
    {
        private static readonly Pool<List<T>, ListPoolPolicy<T>> _shared 
            = Pools.Create<List<T>, ListPoolPolicy<T>>(new ListPoolPolicy<T>(), 64);
        
        public static List<T> Rent() => _shared.Rent();
        
        public static void Return(List<T> list) => _shared.Return(list);
        
        public static PooledScope<List<T>, ListPoolPolicy<T>> RentScoped() 
            => _shared.RentScoped();
        
        public static void Prewarm(int count) => _shared.Prewarm(count);
        
        public static void Clear() => _shared.Clear();
        
#if DEVELOPMENT_BUILD
        public static int RejectedCount => _shared.RejectedCount;
#endif
    }
    
    /// <summary>
    /// 全局共享 Dictionary 池.
    /// </summary>
    public static class DictionaryPool<TKey, TValue> where TKey : notnull
    {
        private static readonly Pool<Dictionary<TKey, TValue>, DictionaryPoolPolicy<TKey, TValue>> _shared 
            = Pools.Create<Dictionary<TKey, TValue>, DictionaryPoolPolicy<TKey, TValue>>(new DictionaryPoolPolicy<TKey, TValue>(), 64);
        
        public static Dictionary<TKey, TValue> Rent() => _shared.Rent();
        public static void Return(Dictionary<TKey, TValue> dict) => _shared.Return(dict);
        public static PooledScope<Dictionary<TKey, TValue>, DictionaryPoolPolicy<TKey, TValue>> RentScoped() 
            => _shared.RentScoped();
        public static void Prewarm(int count) => _shared.Prewarm(count);
        public static void Clear() => _shared.Clear();
        
#if DEVELOPMENT_BUILD
        public static int RejectedCount => _shared.RejectedCount;
#endif
    }
    
    /// <summary>
    /// 全局共享 HashSet 池.
    /// </summary>
    public static class HashSetPool<T>
    {
        private static readonly Pool<HashSet<T>, HashSetPoolPolicy<T>> _shared 
            = Pools.Create<HashSet<T>, HashSetPoolPolicy<T>>(new HashSetPoolPolicy<T>(), 64);
        
        public static HashSet<T> Rent() => _shared.Rent();
        public static void Return(HashSet<T> set) => _shared.Return(set);
        public static PooledScope<HashSet<T>, HashSetPoolPolicy<T>> RentScoped() 
            => _shared.RentScoped();
        public static void Prewarm(int count) => _shared.Prewarm(count);
        public static void Clear() => _shared.Clear();
        
#if DEVELOPMENT_BUILD
        public static int RejectedCount => _shared.RejectedCount;
#endif
    }
    
    /// <summary>
    /// 全局共享 Queue 池.
    /// </summary>
    public static class QueuePool<T>
    {
        private static readonly Pool<Queue<T>, QueuePoolPolicy<T>> _shared 
            = Pools.Create<Queue<T>, QueuePoolPolicy<T>>(new QueuePoolPolicy<T>(), 64);
        
        public static Queue<T> Rent() => _shared.Rent();
        public static void Return(Queue<T> queue) => _shared.Return(queue);
        public static PooledScope<Queue<T>, QueuePoolPolicy<T>> RentScoped() 
            => _shared.RentScoped();
        public static void Prewarm(int count) => _shared.Prewarm(count);
        public static void Clear() => _shared.Clear();
        
#if DEVELOPMENT_BUILD
        public static int RejectedCount => _shared.RejectedCount;
#endif
    }
    
    /// <summary>
    /// 全局共享 Stack 池.
    /// </summary>
    public static class StackPool<T>
    {
        private static readonly Pool<Stack<T>, StackPoolPolicy<T>> _shared 
            = Pools.Create<Stack<T>, StackPoolPolicy<T>>(new StackPoolPolicy<T>(), 64);
        
        public static Stack<T> Rent() => _shared.Rent();
        public static void Return(Stack<T> stack) => _shared.Return(stack);
        public static PooledScope<Stack<T>, StackPoolPolicy<T>> RentScoped() 
            => _shared.RentScoped();
        public static void Prewarm(int count) => _shared.Prewarm(count);
        public static void Clear() => _shared.Clear();
        
#if DEVELOPMENT_BUILD
        public static int RejectedCount => _shared.RejectedCount;
#endif
    }
}