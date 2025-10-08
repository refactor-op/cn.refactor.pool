<div align="center">
  <h1>Refactor Pool</h1>
  <p>
    <img src="https://img.shields.io/badge/Unity-2021.3+-black?logo=unity" />
    <img src="https://img.shields.io/badge/License-MIT-blue.svg" />
    <img src="https://img.shields.io/github/stars/refactor-op/cn.refactor.pool?style=social" />
  </p>
</div>

---

## 特性

- **极致性能** - 单线程 3.7x, 并发 4.1x 性能提升.
- **0 GC** - 百万次调用 0 分配.
- **线程安全** - 三层架构 (ThreadLocal + ConcurrentStack + Policy).
- **策略驱动** - 通过 `IPoolPolicy` 自定义对象生命周期.
- **易用** - 支持回调创建, `using` 语法糖自动归还.

---

## 性能数据

> **测试环境**: Windows 11, i5-13500H 2.60GHz, .NET 9.0

### 单线程

| 实现方式 | 耗时 | 性能提升 | 内存分配 |
|----------|------|----------|----------|
| `new List()` + `Clear()` | 3,525 μs | 基准 | 11.8 MB |
| **对象池 Rent/Return** | **937 μs** | **3.7x ↑** | **0 B ✅** |
| **对象池 Scoped** | **1,502 μs** | **2.3x ↑** | **0 B ✅** |

### 并发 (Parallel.For)

| 实现方式 | 耗时 | 性能提升 | 内存分配 |
|----------|------|----------|----------|
| `new List()` + `Clear()` | 139 μs | 基准 | 1.16 MB |
| **对象池 Rent/Return** | **57 μs** | **2.4x ↑** | **3.4 KB** |
| **对象池 Scoped** | **90 μs** | **1.5x ↑** | **3.4 KB** |

> 池本身 0 分配, 并发环境的 3.4 KB 来自 `Parallel.For` 的任务调度开销.

### 集合

| 集合类型 | 传统方式 | 池化方式 | 性能提升 | 内存节省 |
|----------|---------|---------|---------|---------|
| `List<int>` | 3,300 μs | **955 μs** | **3.5x** | **100%** |
| `Dictionary<int, int>` | 16,907 μs | **6,647 μs** | **2.5x** | **100%** |

---

## 使用

### 1. 定义 Policy

```csharp
public readonly struct ListPolicy : IPoolPolicy<List<int>>
{
    public List<int> Create() => new List<int>();
    public void OnRent(List<int> obj) { }  // 租用时调用.
    public bool OnReturn(List<int> obj)    // 归还时调用.
    {
        obj.Clear();
        return true;  // true = 放回池中, false = 丢弃.
    }
}
```

### 2. 创建并使用

```csharp
// 方式一: 使用 Policy.
var pool = Pools.Default<List<int>, ListPolicy>();

// 方式二: 使用回调.
var pool = Pools.FromCallback(
    factory: () => new List<int>(),
    onReturn: list => { list.Clear(); return true; }
);

// 租用.
var list = pool.Rent();
list.Add(1);
pool.Return(list);

// 自动归还 (推荐).
using (var scoped = pool.RentScoped())
{
    scoped.Value.Add(1);
} // 自动归还.
```

### 3. 预置集合池

```csharp
using Refactor.Pool.Extra;

// 直接使用.
using var list = ListPool<int>.Default.RentScoped();
using var dict = DictionaryPool<string, int>.Default.RentScoped();
using var set = HashSetPool<int>.Default.RentScoped();

list.Value.Add(1);
dict.Value["key"] = 1;
set.Value.Add(1);
```

### 4. 并发环境

```csharp
// 创建线程安全池.
var pool = Pools.Concurrent.Default<List<int>, ListPolicy>();

// 并发使用.
Parallel.For(0, 10000, i =>
{
    using var list = pool.RentScoped();
    list.Value.Add(i);
});
```

### 5. 带参数的 Policy

```csharp
public readonly struct BufferPolicy : IPoolPolicy<byte[], int>
{
    public byte[] Create(int size) => new byte[size];
    public void OnRent(byte[] obj, int size) => Array.Clear(obj, 0, size);
    public bool OnReturn(byte[] obj) => true;
}

// 使用.
var pool = Pools.Default<byte[], int, BufferPolicy>(1024);
var buffer = pool.Rent();  // 获取 1024 字节的缓冲区
```

### 6. 预热与清理

```csharp
var pool = Pools.Default<List<int>, ListPolicy>();

// 预热 (提前创建对象).
pool.Prewarm(100);

// 清空池.
pool.Clear();

// 释放资源.
pool.Dispose();
```

### 7. RAII

```csharp
// ✅ 自动归还.
using (var list = ListPool<int>.Default.RentScoped())
{
    list.Value.AddRange(data);
    Process(list.Value);
}

// ❌ 忘记归还
var list = pool.Rent();
// ... 使用后未归还 = 内存泄漏.
```

## API

### 核心类型

| 类型 | 说明 |
|------|------|
| `Pool<T, TPolicy>` | 单线程对象池 |
| `ConcurrentPool<T, TPolicy>` | 线程安全对象池 |
| `IPoolPolicy<T>` | 对象生命周期策略 |
| `PooledObject<T>` | 自动归还的包装器 (`ref struct`) |

### 静态工厂

| 方法 | 说明 |
|------|------|
| `Pools.Create<T, TPolicy>(...)` | 创建自定义单线程池 |
| `Pools.Default<T, TPolicy>()` | 创建默认单线程池 |
| `Pools.Concurrent.Create<T, TPolicy>(...)` | 创建自定义并发池 |
| `Pools.Concurrent.Default<T, TPolicy>()` | 创建默认并发池 |
| `Pools.FromCallback(...)` | 使用回调创建池 |

### 预置池

| 类型 | 位置 |
|------|------|
| `ListPool<T>` | `Refactor.Pool.Extra` |
| `DictionaryPool<TKey, TValue>` | `Refactor.Pool.Extra` |
| `HashSetPool<T>` | `Refactor.Pool.Extra` |

---

## 贡献

欢迎 PR 和 Issues！