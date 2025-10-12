<div align="center">
  <h1>Refactor Pool</h1>
  <p>
    <img src="https://img.shields.io/badge/Unity-2021.3+-black?logo=unity" />
  </p>
</div>

## 为什么 Refactor.Pool

- 0GC - 存取零分配
- 高性能 - 相比 MS Extensions ObjectPool，性能 5x
- 无锁并发安全 - 基于线程本地存储，多线程环境下对象存取无需加锁

## 快速开始

```csharp
// 定义 Policy.
public readonly struct EnemyPolicy : IPoolPolicy<Enemy>
{
    public Enemy Create() => new Enemy();
    public void OnRent(Enemy obj) => obj.Activate();
    public bool OnReturn(Enemy? obj) => obj?.Deactivate() ?? false;
}

// 使用.
using var enemy = Pools.Create(new EnemyPolicy()).RentScoped();
// ...
// 离开作用域, 自动归还.
// 或者.
var enemy = Pools.Create(new EnemyPolicy()).Rent();
// ...
// 手动归还.
Pools.Create(new EnemyPolicy()).Return(enemy);
```

## Benchmark

> **测试环境**: Windows 11, i5-13500H 2.60GHz, .NET 9.0.6  

### 单线程（10000 次操作）
| 场景 | 耗时 | 性能 | 内存 |
|------|------|----------|----------|
| `new List()` + `Clear()` | 93.32 μs | 基准 | 720 KB |
| **Pool Rent/Return** | **49.78 μs** | **1.87x ↑** | **0 B** |
| **Pool Scoped** | **54.02 μs** | **1.73x ↑** | **0 B** |

### 并发（Parallel.For 10000 次）
| 场景 | 耗时 | 性能 | 内存 |
|------|------|----------|----------|
| `new List()` + `Clear()` | 70.11 μs | 基准 | 725 KB |
| **Pool Rent/Return** | **42.79 μs** | **1.64x ↑** | **4.6 KB** |
| **Pool Scoped** | **43.65 μs** | **1.61x ↑** | **4.6 KB** |
| `Parallel.For` 空循环 | 20.30 μs | - | **4.6 KB** |

> **池本身完全 0 GC**，4.6 KB 来自 TPL 任务调度。

### 与标准库

#### vs Microsoft.Extensions.ObjectPool
| 场景 | 耗时 | 性能 |
|------|------|----------|
| 直接 new | 73.38 μs | 基准 |
| **Refactor Pool** | **33.22 μs** | **2.21x 快** |
| MS Extensions | 166.74 μs | **0.44x 慢** |

#### vs System.Buffers.ArrayPool
| 场景 | 耗时 | 性能 |
|------|------|----------|
| 直接 new | 500.53 μs | 基准 |
| **Refactor Pool** | **108.08 μs** | **4.63x 快** |
| ArrayPool | **32.68 μs** | **15.32x 快** |

> ArrayPool 在数组场景依旧更快。

## 心路历程

[QFramework](https://github.com/liangxiegame/QFramework) 中的 `ListPool<T>` 让我眼前一亮：

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

这段代码十分简洁，一个栈 + 两个方法就把 `List<T>` 的租界与归还做得如此轻盈。相比 QFramework 为其他各种场景设计的复杂池实现，这种不给开发者心智增加负担的设计让我感觉很舒服，于是我想，能不能把他的所有池实现统一起来？

第一步，很自然地想到抽象接口。
```csharp
public interface IPoolable
{
    void OnRent();
    void OnReturn();
}
```

看起来挺标准的。但当我真要池化 `List<T>` 时，尴尬了——我总不能去改 BCL 的代码吧？写个包装类又太丑。所以，接口这条路走不通。

我一度想用源码生成器这个"黑科技"，通过特性在编译时自动生成调用代码。但转念一想，这个"调用者类"到底是什么鬼？本质上不就是把重置逻辑抽出来吗？我完全可以把这东西抽象成策略，而不是硬编码生成。而最适合这个思路的，不就是策略模式嘛。

于是，我定义了 `IPoolPolicy<T>`，让策略来决定对象的创建、清理和回收：
```csharp
public interface IPoolPolicy<T> where T : class
{
    T Create();
    void OnRent(T obj);
    bool OnReturn(T? obj);
}
```

在追求性能的路上，我参考了 Cysharp 的做法。研究 UniTask 源码时，发现它用 struct 实现了 0GC 和高性能。我就想，为什么我们不也用 struct 呢？

现在能用到 struct 的地方就是 PoolPolicy，但它实现了接口，会不会装箱？跟用 class 比起来真有优势吗？我心里没底。为了搞清楚，我查了很多资料，跟社区里的性能优化专家交流，想弄明白到底什么时候该用 struct。

大家告诉我，struct 能内联方法，性能更好。行，那就用 struct，但我得用 benchmark 验证一下——结果证明这个选择是对的。

同时，我还想着要做面向分布式架构的池。大项目肯定要面对并发问题。学过 ET 框架后，我想过用 Actor 或者 Fiber 模型，但这些模型自己也需要池化啊。所以我觉得不能只考虑单线程，得做个并发安全的池，这就是 ConcurrentPool 的由来。

但并发安全怎么做？最常见的办法就是加锁或者用无锁编程。我担心这会影响性能，跑了个 benchmark 一看——果然慢了不少。作为 Cysharp 的粉丝，我寻思肯定有更好的办法。

于是我想到了 Fiber 的设计思路：给每个线程都配个自己的池子？线程安全不一定非要竞争，隔离也行啊！这一刻我明白了：**不要锁！**

每个线程维护自己的本地栈，大部分借还操作都在线程内部完成。但万一线程不够用呢？所以我额外加了个共享池，只有少数对象会进到这里。

接下来该考虑用户体验了。QFramework 的 `ListPool<T>` 用起来真顺手，就像微软的 ArrayPool 一样，用户拿来就用，不用操心创建销毁。这种 API 我必须要有：

```csharp
public static class ListPool<T>
{
    public static readonly Pool<List<T>, ListPolicy.Default<T>> Default
        = Pools.Create(new ListPolicy.Default<T>());
}
```

但策略模式对用户来说有点重，还得额外定义个类。为什么不直接用委托呢？这是我当时很纠结的问题，最后我还是放弃了委托方案。

我试过用委托代替策略，写起来确实漂亮：
```csharp
new Pool<List<int>>(
    factory: () => new List<int>(),
    onRent: list => list.Clear()
);
```

但这个做法把所有为标准库定制的 Policy 都废了，因为它把 Policy 的生态位全占了。最后就会退化成只能用委托建池，但委托调用比 struct 内联方法慢，而且还有别的坑（具体细节我后来忘了，反正 benchmark 显示性能不行）。

我还纠结过要不要支持任意数量的参数，为此写了一堆泛型重载。写到一半实在受不了，突然灵光一现：我干嘛不直接把参数存在 Policy 里呢？struct 又不会分配堆内存。

## 扩展

需要开箱即用的全局池？看看这个 👉 [cn.refactor.pool.extra](https://github.com/refactor-op/cn.refactor.pool.extra)

## 贡献

欢迎 PR & Issue！

## 致谢

Refactor.Pool 的设计受到以下开发者/项目的启发：

- **[Cysharp](https://github.com/Cysharp)**
- **[Ben Adams](https://github.com/benaadams)**
- **[QFramework](https://github.com/liangxiegame/QFramework)**
- **[ET](https://github.com/egametang/ET)**

<div align="center">
  <p><i>Your time is limited, so don't waste it living someone else's life.</i></p>
</div>