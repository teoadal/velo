using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Velo.Benchmark.Dependencies
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    public class DependencyBuildBenchmark
    {
        [Benchmark]
        public string Autofac()
        {
            var builder = DependencyBuilders.ForAutofac();
            var container = builder.Build();

            return container.ToString();
        }

        [Benchmark]
        public string Castle()
        {
            var container = DependencyBuilders.ForCastle();
            return container.ToString();
        }

        [Benchmark(Baseline = true)]
        public string Core()
        {
            var builder = DependencyBuilders.ForCore();
            var container = builder.BuildServiceProvider();

            return container.ToString();
        }

        [Benchmark]
        public string LightInject()
        {
            var container = DependencyBuilders.ForLightInject();
            return container.ToString();
        }
        
        [Benchmark]
        public string SimpleInject()
        {
            var container = DependencyBuilders.ForSimpleInject();
            return container.ToString();
        }
        
        [Benchmark]
        public string Velo()
        {
            var builder = DependencyBuilders.ForVelo();
            var container = builder.BuildContainer();

            return container.ToString();
        }
        
        [Benchmark]
        public string Unity()
        {
            var container = DependencyBuilders.ForUnity();
            return container.ToString();
        }

    }
}