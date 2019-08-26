# Velo
Velo - это набор простейших реализаций известных и часто используемых сервисов для проектов на .NET. Данный репозиторий - набор примеров для информационного портала [habr.com](https://habr.com/ru/users/teoadal/posts), где я публикую статьи. Я стараюсь писать простой код, который понятен, а, главное, релевантен задаче обучения. При этом, каждый раз я пишу тесты и бенчмарки.

В большей части бенчмарков "велосипедный" код несколько обходит по производительности известные решения. Это не значит, что нужно бросаться использовать Velo. Запомните: готовые решения всегда лучше, а данные имплементации могут служить лишь отправной точкой для изучения мапперов, сериализации или DI. Не нужно использовать Velo в production, так как он поставляется "как есть" и содержит самый минимальный набор функциональности, которой достаточно для задачи обучения какой-либо технологии.

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

|              Method | Count |        Mean |      Error |     StdDev | Ratio | RatioSD |     Gen 0 | Gen 1 | Gen 2 |  Allocated |
|-------------------- |------ |------------:|-----------:|-----------:|------:|--------:|----------:|------:|------:|-----------:|
|          **AutoMapper** | **10000** |  **1,267.2 us** |  **10.499 us** |   **8.197 us** |  **1.00** |    **0.00** |  **101.5625** |     **-** |     **-** |   **312.5 KB** |
|    Velo_BasicMapper | 10000 | 12,320.1 us | 116.795 us | 109.250 us |  9.72 |    0.09 | 1109.3750 |     - |     - |  3437.5 KB |
| Velo_CompiledMapper | 10000 |    348.6 us |   4.400 us |   4.116 us |  0.28 |    0.00 |  101.5625 |     - |     - |   312.5 KB |
|                     |       |             |            |            |       |         |           |       |       |            |

## Serialization/Deserialization

#### Десериализация

```cs
var converter = new JConverter();
var deserialized = _converter.Deserialize<Boo[]>(json);
```

#### Сериализация

```cs
var converter = new JConverter();
var json = _converter.Serialize(data);
```

### Benchmark

#### Serialization benchmark

|      Method | Count |     Mean |     Error |    StdDev | Ratio | RatioSD |      Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------ |------ |---------:|----------:|----------:|------:|--------:|-----------:|------:|------:|----------:|
|  **Newtonsoft** | **10000** | **60.63 ms** | **1.5445 ms** | **3.0486 ms** |  **1.00** |    **0.00** |  **8444.4444** |     **-** |     **-** |  **25.44 MB** |
|    FastJson | 10000 | 61.34 ms | 0.8764 ms | 0.8198 ms |  0.99 |    0.06 | 20222.2222 |     - |     - |  60.75 MB |
| Simple_Json | 10000 | 65.97 ms | 1.1127 ms | 1.0408 ms |  1.06 |    0.06 | 23875.0000 |     - |     - |     72 MB |
|    SpanJson | 10000 | 16.57 ms | 0.1236 ms | 0.1095 ms |  0.27 |    0.01 |  1437.5000 |     - |     - |    4.4 MB |
|        Velo | 10000 | 21.48 ms | 0.1737 ms | 0.1625 ms |  0.35 |    0.02 |  1968.7500 |     - |     - |   5.93 MB |
|             |       |          |           |           |       |         |            |       |       |           |

#### Deserialization benchmark

|      Method | Count |      Mean |     Error |    StdDev | Ratio | RatioSD |       Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------ |------ |----------:|----------:|----------:|------:|--------:|------------:|------:|------:|----------:|
|  **Newtonsoft** | **10000** |  **77.19 ms** | **1.1383 ms** | **1.0648 ms** |  **1.00** |    **0.00** |  **11714.2857** |     **-** |     **-** |  **35.47 MB** |
|    FastJson | 10000 |  75.51 ms | 1.1730 ms | 1.0973 ms |  0.98 |    0.02 |  16142.8571 |     - |     - |  48.81 MB |
| Simple_Json | 10000 | 143.76 ms | 2.8330 ms | 2.5114 ms |  1.86 |    0.03 | 222000.0000 |     - |     - | 667.02 MB |
|    SpanJson | 10000 |  15.48 ms | 0.2174 ms | 0.2033 ms |  0.20 |    0.00 |    906.2500 |     - |     - |   2.75 MB |
|        Velo | 10000 |  41.96 ms | 0.5851 ms | 0.5473 ms |  0.54 |    0.01 |   4076.9231 |     - |     - |  12.36 MB |
|             |       |           |           |           |       |         |             |       |       |           |


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

### Resolve dependency

```cs
var repositories = container.Resolve<IRepository[]>();
var session = container.Resolve<ISession>();
var otherSession = container.Resolve<ISession>();
var converterSingleton = container.Resolve<JConverter>();
```

### Benchmarks

#### Create container benchmark

|       Method |       Mean |     Error |    StdDev |  Ratio | RatioSD |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
|------------- |-----------:|----------:|----------:|-------:|--------:|--------:|-------:|------:|----------:|
|      Autofac |  44.170 us | 0.8341 us | 0.9930 us |  18.38 |    0.63 | 10.5591 |      - |     - |  32.46 KB |
|       Castle | 300.474 us | 4.3010 us | 4.0232 us | 123.92 |    4.36 | 27.3438 |      - |     - |  84.32 KB |
|         Core |   2.410 us | 0.0481 us | 0.0842 us |   1.00 |    0.00 |  1.1520 |      - |     - |   3.55 KB |
|  LightInject |  12.365 us | 0.2160 us | 0.2020 us |   5.10 |    0.19 |  7.7209 | 0.0153 |     - |  23.74 KB |
| SimpleInject | 362.265 us | 7.3623 us | 8.7643 us | 150.73 |    5.26 | 12.2070 | 5.8594 |     - |  37.44 KB |
|         Velo |   1.407 us | 0.0251 us | 0.0234 us |   0.58 |    0.02 |  0.7725 |      - |     - |   2.38 KB |
|        Unity |  22.492 us | 0.3862 us | 0.3613 us |   9.27 |    0.24 |  6.9275 |      - |     - |  21.34 KB |

#### Resolve dependency from container

|       Method |      Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------- |----------:|----------:|----------:|------:|--------:|-------:|------:|------:|----------:|
|      Autofac | 906.55 ns |  8.382 ns |  7.430 ns |  5.74 |    0.05 | 0.5512 |     - |     - |    1736 B |
|       Castle | 914.03 ns | 14.751 ns | 12.318 ns |  5.78 |    0.09 | 0.3986 |     - |     - |    1256 B |
|         Core | 158.08 ns |  1.671 ns |  1.396 ns |  1.00 |    0.00 | 0.0710 |     - |     - |     224 B |
|  LightInject |  89.96 ns |  1.370 ns |  1.281 ns |  0.57 |    0.01 | 0.0712 |     - |     - |     224 B |
| SimpleInject | 138.69 ns |  2.739 ns |  2.562 ns |  0.88 |    0.02 | 0.0710 |     - |     - |     224 B |
|         Velo | 143.04 ns |  2.901 ns |  4.429 ns |  0.92 |    0.03 | 0.0710 |     - |     - |     224 B |
|        Unity | 419.65 ns |  5.223 ns |  4.630 ns |  2.65 |    0.03 | 0.1779 |     - |     - |     560 B |
