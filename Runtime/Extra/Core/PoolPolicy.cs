using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Refactor.Pooling
{
    public readonly struct ListPoolPolicy<T> : IPoolPolicy<List<T>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly List<T> Create() => new List<T>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void OnRent(List<T> obj) { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool OnReturn(List<T> obj)
        {
            if (obj == null) return false;
            obj.Clear();
            return true;
        }
    }
    
    public readonly struct DictionaryPoolPolicy<TKey, TValue> : IPoolPolicy<Dictionary<TKey, TValue>>
        where TKey : notnull
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Dictionary<TKey, TValue> Create() => new Dictionary<TKey, TValue>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void OnRent(Dictionary<TKey, TValue> obj) { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool OnReturn(Dictionary<TKey, TValue> obj)
        {
            if (obj == null) return false;
            obj.Clear();
            return true;
        }
    }
    
    public readonly struct HashSetPoolPolicy<T> : IPoolPolicy<HashSet<T>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly HashSet<T> Create() => new HashSet<T>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void OnRent(HashSet<T> obj) { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool OnReturn(HashSet<T> obj)
        {
            if (obj == null) return false;
            obj.Clear();
            return true;
        }
    }
    
    public readonly struct QueuePoolPolicy<T> : IPoolPolicy<Queue<T>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Queue<T> Create() => new Queue<T>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void OnRent(Queue<T> obj) { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool OnReturn(Queue<T> obj)
        {
            if (obj == null) return false;
            obj.Clear();
            return true;
        }
    }
    
    public readonly struct StackPoolPolicy<T> : IPoolPolicy<Stack<T>>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Stack<T> Create() => new Stack<T>();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void OnRent(Stack<T> obj) { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool OnReturn(Stack<T> obj)
        {
            if (obj == null) return false;
            obj.Clear();
            return true;
        }
    }
}