# Refactor.Pooling

通过 struct 策略消除虚调用，用固定容量保证确定性，实现高性能、零分配池化。

## 基准测试

> 环境：Windows 11，.NET 8，BenchmarkDotNet v0.13.12

### Struct Policy vs 其他实现

| 策略类型 | 单次操作 | 100 次操作 | 1000 次操作 | 开销倍数 |
|----------|----------|------------|-------------|----------|
| **Struct Policy** | **3.3 ns** | **285 ns** | **4.8 μs** | **1.0×** |
| Class Policy | 6.2 ns | 661 ns | 8.8 μs | 1.8–2.3× |
| Interface Policy | 9.4 ns | 1,063 ns | 11.4 μs | 2.8–3.7× |

### vs Microsoft.Extensions.ObjectPool

| 场景 | 本库 (1/100 次) | 微软 (1/100 次) | 性能倍数 | 分配 (100 次) |
|------|-----------------|-----------------|----------|---------------|
| 简单对象 | 3.44 ns / 217.7 ns | 14.73 ns / 1601.6 ns | **4.3× / 7.4×** | 0 B / ~1592 B |
| List\<int\> | 3.41 ns / 289.5 ns | 14.55 ns / 1755.6 ns | **4.3× / 6.1×** | 0 B / ~1592 B |
| StringBuilder | 4.41 ns / 411.0 ns | 15.08 ns / 1662.6 ns | **3.4× / 4.0×** | 0 B / ~412 B |

## 哲学

笔者在使用 QFramework 开发游戏时，意外地撞见了其 `ListPool<T>`，不禁让我眼前一亮：

```csharp
public static class ListPool<T>
{
    static Stack<List<T>> mListStack = new Stack<List<T>>();
    public static List<T> Get() =>
        mListStack.Count == 0 ? new List<T>() : mListStack.Pop();
    public static void Release(List<T> list)
    {
        list.Clear();
        mListStack.Push(list);
    }
}
```

一个栈加两个方法，就把 `List<T>` 的租借与归还做到了如此美丽且简洁，它不增加开发者心智负担，却解决了 List 的复用问题。

彼时我追求一致性以至疯魔，QFramework 提供了服务不同场景的池，使我抓狂：要是只需要一个简单的入口，就将服务于各种场景的池统一起来就好了！

第一个念头自然是接口——让被池化的对象实现生命周期：

```csharp
public interface IPoolable
{
    void OnRent();
    void OnReturn();
}
```

但很快就卡住了：我们同时还需要池化如 `List<T>`，我既不能修改它们，也不想用丑陋的包装类。**接口这条路行不通。**

我开始考虑源码生成器，通过 Attribute 驱动，在编译时生成重置调用。但这看似可行，本质仍是在产出一个调用者类/方法——那么这不就是策略模式吗：

```csharp
public interface IPoolPolicy<T>
{
    T Create();           // 创建对象.
    void OnRent(T obj);   // 激活对象.
    bool OnReturn(T obj); // 清理并检查, false = 拒绝归还.
}
```

**策略模式将生命周期逻辑"外包"出去，完美解决了池化万物的问题。**

恰逢学习 UniTask 的源码，其 `struct` 的设计启发了我：实现了接口的 `struct` 策略在热路径下可以消除虚调用、利于 JIT 内联，并以零分配和更少分支让性能更稳定。

**于是有了 struct 策略，比委托快 4.5 倍。**

我试过用委托代替策略，写起来确实漂亮，但委托会直接侵占策略模式的生态位，将策略的性能优势完全抹除，所以我放弃了委托方案。

接下来面临一个经典问题：要不要自动扩容？

**内存管理成本无法消除，只能转移。** 经过分析：
- **丢弃**的 GC 成本发生在 Return，可预测（`_ptr < _storage.Length`），耗时固定
- **扩容**的 `new T[] + Array.Copy` 耗时与容量成正比，且不可预测

**我们选择固定容量，换取确定性性能。**

```csharp
public class Pool<T>
{
    private readonly T[] _storage;  // 固定大小.
    public void Return(T obj)
    {
        if (_ptr < _storage.Length) 
            _storage[_ptr++] = obj;
        // else 丢弃，让 GC 处理.
    }
}
```

结果：游戏跑起来内存很稳定，但当池满时会丢弃对象，导致 GC 频繁——这暗示我们 capacity 设置得不对。

于是我们加入了统计功能：

```csharp
#if DEVELOPMENT_BUILD
private int _rejectedCount;
#endif

public void Return(T obj)
{
    if (_ptr < _storage.Length) 
        _storage[_ptr++] = obj;
#if DEVELOPMENT_BUILD
    else 
        _rejectedCount++;  // 记录拒绝次数.
#endif
}
```

**统计功能用 `DEVELOPMENT_BUILD` 保护，零生产成本，但能帮助我们调优容量。**

另外，池化对象的归还常常被遗忘，这源自事件注销的痛点。我们引入了 `RentScoped()`：

```csharp
public ref struct PooledScope<T>
{
    public void Dispose()
    {
        if (_value != null) _pool.Return(_value);
    }
}

// 使用.
using var scope = pool.RentScoped();
scope.Value.DoSomething();
// 离开作用域自动归还.
```

**RAII 让归还变得自动且安全。**

是时候池化 BCL 了。但问题来了：当某次任务使用的 `List<T>` 多次扩容后，其容量会增加。不调用 `TrimExcess()` 是不会缩容的，如果这种现象持续，`List<T>` 只会越来越大。

**我一度陷入了"池应该管理对象容量"的陷阱。**

我尝试过：
- 判断对象大小，拒绝超过 85KB 的对象（LOH）
- Expression Trees（反射访问内部容量字段）
- 魔法代码（各种黑科技）

但最终醒悟：**这些都是错的。**

```csharp
// 用户每次需要的容量不同.
void Task1() 
{
    var list = pool.Rent();  // Capacity = 4
    for (int i = 0; i < 1000; i++) list.Add(item);  // 扩容到 1024.
    pool.Return(list);  // 拒绝（太大）-> GC.
}

void Task2()
{
    var list = pool.Rent();  // 又是 Capacity = 4.
    for (int i = 0; i < 1000; i++) list.Add(item);  // 又扩容.
    pool.Return(list);  // 又拒绝 -> 又 GC.
}

// 结果: 池化完全失效.
```

**池化的目的是复用对象，容量检查导致不断拒绝，反而破坏了复用。**

经过大量讨论和反思，我理解了池化的本质：

> **池化 = 管理对象生命周期，不是管理对象内部状态。**

**容量是对象的内部状态，应该由使用者决定，而非池。**

对于容量问题，正确的解决方案是：
- **小数据**：接受第一次扩容，后续容量稳定，零分配
- **大数据**：不要用 `List`，用 `ArrayPool` 或 `NativeArray`（容量固定）
- **不可预测**：不应该池化

**最终，我们删除了所有容量检查。Policy 变得极简：**

```csharp
public readonly struct ListPoolPolicy<T> : IPoolPolicy<List<T>>
{
    public List<T> Create() => new List<T>();
    public void OnRent(List<T> obj) { }
    public bool OnReturn(List<T> obj)
    {
        if (obj == null) return false;
        obj.Clear();
        return true;  // 总是接受.
    }
}
```

## 使用

```csharp
// 静态池.
ListPool<T>.Rent()
ListPool<T>.Return(list)
ListPool<T>.RentScoped()

// Dictionary, HashSet, Queue, Stack ...

// 创建.
var pool = Pools.Create(policy, capacity);

// 租借.
var obj = pool.Rent();
using var scope = pool.RentScoped();

// 归还.
pool.Return(obj);

// 工具.
pool.Prewarm(count);  // 预热.
pool.Clear();         // 清空.

#if DEVELOPMENT_BUILD
pool.RejectedCount    // 拒绝次数.
#endif
```

## 许可证

MIT License

## 贡献

欢迎 Issue 与 PR。