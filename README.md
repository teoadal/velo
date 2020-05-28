# Velo

[![.NET Core](https://github.com/teoadal/velo/workflows/.NET%20Core/badge.svg?branch=master)](https://github.com/teoadal/velo/actions)
[![codecov](https://codecov.io/gh/teoadal/velo/branch/master/graph/badge.svg)](https://codecov.io/gh/teoadal/velo) 
[![NuGet](https://img.shields.io/nuget/v/velo.svg)](https://www.nuget.org/packages/velo) 
[![NuGet](https://img.shields.io/nuget/dt/velo.svg)](https://www.nuget.org/packages/velo)

Performance oriented small library with simple implementations of popular patterns and workflows: CQRS, IoC, serialization/deserialization, logging and mapping. *Under construction*.

## Install from nuget

Install Velo with the following command [from nuget](https://www.nuget.org/packages/Velo/):

```ini
Install-Package Velo
```

[For integration](https://github.com/teoadal/velo/wiki/Integration-with-IServiceCollection) with IServiceCollection, install Velo.Extensions.DependencyInjection [from nuget](https://www.nuget.org/packages/Velo.Extensions.DependencyInjection):

```ini
Install-Package Velo.Extensions.DependencyInjection
```

## Emitter (mediator)

```cs
var dependencyProvider = new DependencyCollection()
    .AddEmitter()
    .Scan(scanner => scanner // collect all processors and behaviours
        .AssemblyOf<IBooRepository>()
        .AddEmitterProcessors())     
    .BuildProvider();

var emitter = dependencyProvider.GetRequiredService<Emitter>();

// ask query (send request)
Boo boo = await emitter.Ask(new GetBoo { Id = id }); 

// execute command
await emitter.Execute(new CreateBoo { Id = id }); 

// publish notification
await emitter.Publish(new Notification { Created = true });

// or ask as struct for reduce memory traffic
Boo boo = await emitter.Ask<GetBooStruct, Boo>(new GetBooStruct {Id = id}); 
```

### Registration

```cs
var dependencyProvider = new DependencyCollection()
    .AddCommandBehaviour<MeasureBehaviour>() // add behaviours
    .AddCommandProcessor<PreProcessor>()
    .AddCommandProcessor<Processor>(DependencyLifetime.Scoped) 
    .AddCommandProcessor<PostProcessor>()
    .AddQueryProcessor<QueryPreProcessor>()
    .AddQueryProcessor<QueryProcessor>()
    .AddQueryProcessor<QueryPostProcessor>()
    .AddNotificationProcessor<OnBooCreated>()
```

### Mediator query (request) benchmark (per 1000 requests)

|               Method |      Mean |    Error |   StdDev | Ratio |  Allocated |
|--------------------- |----------:|---------:|---------:|------:|-----------:|
| FullPipeline_MediatR | 805,866.1 ns | 15,881.23 ns | 21,738.41 ns |  1.00 | 1056.07 KB |
|    **FullPipeline_Velo** | 272,772.9 ns |  5,236.39 ns |  6,430.75 ns |  **0.34** |  **256.07 KB** |
|                      |              |              |              |       |           |
|  One_Request_MediatR | 358,579.4 ns |  6,727.49 ns |  7,198.33 ns |  1.00 |  376.07 KB |
|     **One_Request_Velo** | 132,259.3 ns |  2,136.00 ns |  1,998.02 ns |  **0.37** |  **144.07 KB** |

*FullPipeline - behaviour, pre- and post-processor*.

### Mediator notification benchmark (per 1000 notifications)

|  Method |     Mean |   Error |  StdDev | Ratio | Allocated |
|-------- |---------:|--------:|--------:|------:|----------:|
| MediatR | 469.6 us | 9.69 us | 8.59 us |  1.00 |  776 074 B |
| **Velo** | 107.4 us | 1.63 us | 1.44 us |  **0.23** |      **72 B** |

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

### Benchmark (per 10000 objects)

|     Method |        Mean |     Error |    StdDev | Ratio |  Allocated |
|----------- |------------:|----------:|----------:|------:|-----------:|
| AutoMapper |    998.9 us |  10.17 us |   9.51 us |  1.00 |  390.63 KB |
| **Velo**   |    299.7 us |   3.06 us |   2.86 us |  **0.30** |  **390.63 KB** |


## Serialization/Deserialization

### Deserialization

```cs
var converter = new JConverter();
var deserialized = converter.Deserialize<Boo[]>(json);
```

### Serialization

```cs
var converter = new JConverter();
var json = converter.Serialize(data);
```

### Serialization benchmark (per 10000 objects)

|     Method |     Mean |    Error |   StdDev | Ratio | Allocated |
|----------- |---------:|---------:|---------:|------:|----------:|
| Newtonsoft | 54.83 ms | 0.567 ms | 0.531 ms |  1.00 |  32.71 MB |
|   **Velo** | 28.71 ms | 0.431 ms | 0.404 ms |  **0.52** |   **9.87 MB** |


### Deserialization benchmark (per 10000 objects)

|     Method |     Mean |    Error |   StdDev | Ratio | Allocated |
|----------- |---------:|---------:|---------:|------:|----------:|
| Newtonsoft | 88.41 ms | 1.575 ms | 1.315 ms |  1.00 |  36.85 MB |
|   **Velo** | 44.28 ms | 0.327 ms | 0.306 ms |  **0.50** |  **19.26 MB** |


## Logger 

### Configure logger

```cs
var container = new DependencyCollection()
    .AddLogger()
    .AddDefaultLogEnrichers()         // log level, sender, timestamp
    .AddLogEnricher<Enricher>()       // add your enricher 
    .AddLogWriter<ConsoleLogWriter>() // included primitive log writer
    .AddLogWriter<LogWriter>()        // add your writer
```

### Use logger

```cs
var logger = _dependencyProvider.GetRequiredService<ILogger<MyClass>>();
logger.Debug("My code for handling {instance} executed at {elapsed}", instance, timer.Elapsed);
```

### Structured logging

```ini
[DBG] [MyClass] [2020-02-26T17:07:57] My code for handling { "Id": 129, "Bool": true, "Double": 61, "Float": 198, "Int": 11, "IntNullable": 177, "String": "String64630110-c7c9-4ba2-94c0-4c28dd9cea20", "Type": 0, "Values": [36,17,212] } executed at "0:00:00.0000025".
```

### Logger benchmark (per 1000 log events)

|         Method |       Mean |    Error |   StdDev | Ratio |  Allocated |
|--------------- |-----------:|---------:|---------:|------:|-----------:|
|   Serilog_EmptySink | 1,110.0 us | 21.86 us | 23.39 us |  1.00 |  851.56 KB |
|      Nlog_EmptyTarget | 2,355.9 us | 44.31 us | 47.41 us |  2.12 | 1 911.15 KB |
|      **Velo_EmptyWriter** |   902.4 us | 12.68 us | 11.86 us |  **0.82** |  **219.25 KB** |
|                |            |          |          |       |            |
| Serilog_StringWriter | 1,439.7 us | 14.88 us | 12.42 us |  1.00 |  976.25 KB |
|    Nlog_StringWriter | 2,061.0 us | 26.41 us | 24.70 us |  1.43 | 1 648.44 KB |
|    **Velo_StringWriter** | 1,225.1 us | 11.20 us |  9.93 us |  **0.85** |  **219.56 KB** |

## Dependency Injection

### Create dependency provider

```cs
var container = new DependencyCollection()
    .AddScoped<SomethingController>()
    .AddSingleton<IFooService, FooService>()
    .AddSingleton(typof(IMapper<>), typeof(CompiledMapper<>))
    .AddSingleton<IConfiguration>(ctx => new Configuration())
    .AddTransient<ISession, Session>()
    .BuildProvider();
```

### Use assembly scanner for find generic interface implementations

```cs
var provider = new DependencyCollection()
    .Scan(scanner => scanner
        .AssemblyOf<IRepository>()
        .SingletoneOf(typeof(IRepository<>)))
    .BuildProvider();
```

### Resolve dependency

```cs
// possible null or empty
var repositoryArray = provider.Get<IRepository[]>();

// not null or exception
var converterSingleton = provider.GetRequired<JConverter>();

// registered as transient
var session = container.Get<ISession>();
var otherSession = container.Get<ISession>();
```

### Use scope

```cs
using (var scope = provider.StartScope())
{
    var controller = scope.Get<SomethingController>();
}
```

### Benchmarks

#### Create dependency container benchmark

|       Method |       Mean |     Error |    StdDev |  Ratio | Allocated |
|------------- |-----------:|----------:|----------:|-------:|----------:|
|      Autofac |  42.710 us | 0.4666 us | 0.4136 us |  18.28 |   38.9 KB |
|       Castle | 245.270 us | 2.0700 us | 1.9363 us | 105.06 |  91.71 KB |
|         Core |   2.338 us | 0.0254 us | 0.0212 us |   1.00 |   5.54 KB |
|  LightInject |  13.078 us | 0.0809 us | 0.0757 us |   5.60 |  37.45 KB |
| SimpleInject | 401.719 us | 3.0091 us | 2.6675 us | 171.80 |   42.7 KB |
|     **Velo** |   1.739 us | 0.0200 us | 0.0178 us |   **0.74** |   **3.04 KB** |
|        Unity |  15.096 us | 0.2847 us | 0.2796 us |   6.44 |  22.41 KB |


#### Resolve singleton service from container

|       Method |        Mean |     Error |    StdDev | Ratio | Allocated |
|------------- |------------:|----------:|----------:|------:|----------:|
|      Autofac |   762.68 ns | 15.218 ns | 25.841 ns |  3.83 |   1 656 B |
|       Castle |   536.30 ns |  5.416 ns |  5.066 ns |  2.71 |   1 200 B |
|         Core |   198.20 ns |  2.320 ns |  2.057 ns |  1.00 |     216 B |
|  LightInject |    85.05 ns |  0.624 ns |  0.584 ns |  0.43 |     216 B |
| SimpleInject |   128.19 ns |  0.942 ns |  0.881 ns |  0.65 |     216 B |
|     **Velo** |   195.92 ns |  2.089 ns |  1.954 ns |  **0.99** |     **216 B** |
|        Unity |   455.58 ns |  3.135 ns |  2.932 ns |  2.30 |     552 B |


## LocalVector (small collection on stack)

Ref struct for collect values on stack. This collection allows you to reduce memory consumption. Also, it allows you to work with several variables as a collection without extra costs. Read about LocalList [here](https://github.com/teoadal/velo/wiki/LocalList-(collection-on-stack)).

### Usage

```cs
var vector = new LocalVector<Boo>();
vector.Add(new Boo()); // add less 10 elements for performance effect
```

### Linq-like via ref struct enumerators

```cs
var outer = new LocalVector<Boo>(_items);
var inner = new LocalVector<Boo>(_reversItems);

var counter = 0;
foreach (var number in outer
    .Join(inner, o => o, i => i, (o, i) => i) 
    .Where((b, threshold) => b.Int > threshold, _threshold) // use argument for avoid clousure
    .Select((b, modifier) => b.Id * modifier, _modifier)
    .OrderBy(id => id))
{
    counter += number;
}

return counter;
```

### Local vector benchmarks (collection with 10 elements)

|                    Method |         Mean |      Error |     StdDev | Ratio | Allocated |
|-------------------------- |-------------:|-----------:|-----------:|------:|----------:|
|                  List_Add |    12.926 ns |  0.1850 ns |  0.1731 ns |  1.00 |      33 B |
|           LocalVector_Add |     5.502 ns |  0.0313 ns |  0.0244 ns |  0.42 |         - |
|                  Span_Add |     3.852 ns |  0.0196 ns |  0.0153 ns |  0.30 |      10 B |
|                           |              |            |            |       |           |
|            List_Iteration |     9.937 ns |  0.2311 ns |  0.2923 ns |  1.00 |      14 B |
|     LocalVector_Iteration |     9.701 ns |  0.0504 ns |  0.0472 ns |  0.97 |         - |
|            Span_Iteration |     8.521 ns |  0.0899 ns |  0.0797 ns |  0.85 |      10 B |
|                           |              |            |            |       |           |
|              List_GroupBy |    71.959 ns |  0.5214 ns |  0.4877 ns |  1.00 |     102 B |
|       LocalVector_GroupBy |    83.630 ns |  0.2430 ns |  0.2273 ns |  1.16 |         - |
|                           |              |            |            |       |           |
|                 List_Join |   111.531 ns |  0.3870 ns |  0.3232 ns |  1.00 |     167 B |
|          LocalVector_Join |   117.925 ns |  0.3964 ns |  0.3708 ns |  1.06 |         - |
|                           |              |            |            |       |           |
|             List_ManyLinq |   238.136 ns |  1.0246 ns |  0.9584 ns |  1.00 |     366 B |
|      LocalVector_ManyLinq |   425.516 ns |  1.3847 ns |  1.2952 ns |  1.79 |         - |
|                           |              |            |            |       |           |
|               List_Remove |    31.604 ns |  0.2091 ns |  0.1956 ns |  1.00 |      14 B |
|        LocalVector_Remove |    52.423 ns |  0.3658 ns |  0.3422 ns |  1.66 |         - |
|                           |              |            |            |       |           |
|               List_Select |    22.865 ns |  0.0749 ns |  0.0664 ns |  1.00 |      21 B |
|        LocalVector_Select |    17.396 ns |  0.0883 ns |  0.0737 ns |  0.76 |         - |
|                           |              |            |            |       |           |
|              List_ToArray |     8.174 ns |  0.0553 ns |  0.0461 ns |  1.00 |      24 B |
|       LocalVector_ToArray |    11.211 ns |  0.0623 ns |  0.0583 ns |  1.37 |      10 B |
|              Span_ToArray |     2.284 ns |  0.0195 ns |  0.0182 ns |  0.28 |      10 B |
|                           |              |            |            |       |           |
|                List_Where |    22.720 ns |  0.0935 ns |  0.0874 ns |  1.00 |      21 B |
|              List_FindAll |    20.528 ns |  0.1968 ns |  0.1744 ns |  0.90 |      31 B |
|         LocalVector_Where |    18.991 ns |  0.0860 ns |  0.0805 ns |  0.84 |         - |

