using BenchmarkDotNet.Running;

using Velo.Benchmark.Dependencies;

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
        }
    }
}