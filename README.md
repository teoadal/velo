# Velo

Velo - это набор простейших реализаций известных и часто используемых сервисов для проектов на .NET. Данный репозиторий - набор примеров для информационного портала [habr.com](https://habr.com/ru/users/teoadal/posts), где я публикую статьи. Я стараюсь писать простой код, который понятен, а, главное, релевантен задаче обучения. При этом, каждый раз я пишу тесты и бенчмарки.

В большей части бенчмарков "велосипедный" код несколько обходит по производительности известные решения. Это не значит, что нужно бросаться использовать Velo. Запомните: готовые решения всегда лучше, а данные имплементации могут служить лишь отправной точкой для изучения мапперов, сериализации или DI. Не нужно использовать Velo в production, так как он поставляется "как есть" и содержит самый минимальный набор функциональности, которой, впрочем, достаточно для задачи обучения.

Я вас предупредил.

## Mapper

```cs
var compiledMapper = new CompiledMapper<Foo>();

var source = new Boo
{
    Bool = true,
    Float = 1f,
    Int = 11
};

var foo = compiledMapper.Map(source);
```

### Benchmark

|              Method |        Mean |      Error |     StdDev | Ratio |  Allocated |
|-------------------- |------------:|-----------:|-----------:|------:|-----------:|
|          AutoMapper |  1,267.2 us |  10.499 us |   8.197 us |  1.00 |   312.5 KB |
|                Velo |    348.6 us |   4.400 us |   4.116 us |  0.28 |   312.5 KB |

## Serialization/Deserialization

### Десериализация

```cs
var converter = new JConverter();
var deserialized = _converter.Deserialize<Boo[]>(json);
```

### Сериализация

```cs
var converter = new JConverter();
var json = _converter.Serialize(data);
```

### Serialization benchmark

|      Method |     Mean |     Error |    StdDev | Ratio | Allocated |
|------------ |---------:|----------:|----------:|------:|----------:|
|  **Newtonsoft** | 60.63 ms | 1.5445 ms | 3.0486 ms |  **1.00** |  **25.44 MB** |
|    FastJson | 61.34 ms | 0.8764 ms | 0.8198 ms |  0.99 |  60.75 MB |
| Simple_Json | 65.97 ms | 1.1127 ms | 1.0408 ms |  1.06 |     72 MB |
|    SpanJson | 16.57 ms | 0.1236 ms | 0.1095 ms |  0.27 |    4.4 MB |
|        **Velo** | 21.48 ms | 0.1737 ms | 0.1625 ms |  **0.35** |   **5.93 MB** |

### Deserialization benchmark

|      Method |      Mean |     Error |    StdDev | Ratio | Allocated |
|------------ |----------:|----------:|----------:|------:|----------:|
|  **Newtonsoft** |  77.19 ms | 1.1383 ms | 1.0648 ms |  **1.00** |  **35.47 MB** |
|    FastJson |  75.51 ms | 1.1730 ms | 1.0973 ms |  0.98 |  48.81 MB |
| Simple_Json | 143.76 ms | 2.8330 ms | 2.5114 ms |  1.86 | 667.02 MB |
|    SpanJson |  15.48 ms | 0.2174 ms | 0.2033 ms |  0.20 |   2.75 MB |
|        **Velo** |  41.96 ms | 0.5851 ms | 0.5473 ms |  **0.54** |  **12.36 MB** |


## Dependency Injection

### Create container

```cs
var container = new DependencyBuilder()
    .AddFactory<ISession, Session>()
    .AddSingleton<JConverter>("converter")
    .AddSingleton<IConfiguration>(ctx => new Configuration())
    .Configure(dataRepository => dataRepository
        .Contracts<IRepository, IFooRepository>()
        .Implementation<FooRepository>()
        .Singleton())
    .Configure(userRepository => userRepository
        .Contracts<IRepository, IBooRepository>()
        .Implementation<BooRepository>()
        .Singleton())
    .AddScope<SomethingController>()
    .BuildContainer();
```

### Use assembly scanner for find generic interface implementations

```cs
var container = new DependencyBuilder()
    .AddSingleton<IConfiguration, Configuration>()
    .AddFactory<ISession, Session>()
    .AddSingleton<JConverter>()
    .Scan(scanner => scanner
        .Assembly(typeof(IRepository).Assembly)
        .RegisterGenericInterfaceAsSingleton(typeof(IRepository<>)))
    .BuildContainer();
```

### Resolve dependency

```cs
var repositories = container.Resolve<IRepository[]>();
var session = container.Resolve<ISession>();
var otherSession = container.Resolve<ISession>(); // registered as Factory
var converterSingleton = container.Resolve<JConverter>();
```

### Use scope

```cs
using (container.StartScope())
{
    var controller = container.Resolve<SomethingController>();
}
```

### Benchmarks

#### Create container benchmark

|       Method |       Mean |     Error |    StdDev |  Ratio | Allocated |
|------------- |-----------:|----------:|----------:|-------:|----------:|
|      Autofac |  44.170 us | 0.8341 us | 0.9930 us |  18.38 |  32.46 KB |
|       Castle | 300.474 us | 4.3010 us | 4.0232 us | 123.92 |  84.32 KB |
|         **Core** |   2.410 us | 0.0481 us | 0.0842 us |   **1.00** |   **3.55 KB** |
|  LightInject |  12.365 us | 0.2160 us | 0.2020 us |   5.10 |  23.74 KB |
| SimpleInject | 362.265 us | 7.3623 us | 8.7643 us | 150.73 |  37.44 KB |
|         **Velo** |   1.407 us | 0.0251 us | 0.0234 us |   **0.58** |   **2.38 KB** |
|        Unity |  22.492 us | 0.3862 us | 0.3613 us |   9.27 |  21.34 KB |

#### Resolve dependency from container

|       Method |      Mean |     Error |    StdDev | Ratio | Allocated |
|------------- |----------:|----------:|----------:|------:|----------:|
|      Autofac | 906.55 ns |  8.382 ns |  7.430 ns |  5.74 |    1736 B |
|       Castle | 914.03 ns | 14.751 ns | 12.318 ns |  5.78 |    1256 B |
|         **Core** | 158.08 ns |  1.671 ns |  1.396 ns |  **1.00** |     **224 B** |
|  LightInject |  89.96 ns |  1.370 ns |  1.281 ns |  0.57 |     224 B |
| SimpleInject | 138.69 ns |  2.739 ns |  2.562 ns |  0.88 |     224 B |
|         **Velo** | 143.04 ns |  2.901 ns |  4.429 ns |  **0.92** |     **224 B** |
|        Unity | 419.65 ns |  5.223 ns |  4.630 ns |  2.65 |     560 B |
