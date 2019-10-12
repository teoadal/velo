using BenchmarkDotNet.Running;
using Velo.Benchmark.Dependencies;
using Velo.Benchmark.Mediators;
using Velo.Benchmark.Serialization;

namespace Velo.Benchmark
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<MappersBenchmark>();

            BenchmarkRunner.Run<DeserializationBenchmark>();
            BenchmarkRunner.Run<SerializationBenchmark>();

            BenchmarkRunner.Run<DependencyBuildBenchmark>();
            BenchmarkRunner.Run<DependencyResolveBenchmark>();

            BenchmarkRunner.Run<MediatorRequestBenchmark>();
        }
    }
}