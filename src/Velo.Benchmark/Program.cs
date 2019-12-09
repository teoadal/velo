using BenchmarkDotNet.Running;
using Velo.Benchmark.Collections;
using Velo.Benchmark.CQRS;
using Velo.Benchmark.DependencyInjection;
using Velo.Benchmark.Serialization;

namespace Velo.Benchmark
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            RunCollections();
            RunCQRS();
            RunDependencyInjection();
            RunMappers();
            RunSerialization();
        }

        private static void RunCollections()
        {
            BenchmarkRunner.Run<LocalVectorBenchmark>();
        }

        // ReSharper disable once InconsistentNaming
        public static void RunCQRS()
        {
            BenchmarkRunner.Run<MediatorRequestBenchmark>();
        }

        public static void RunDependencyInjection()
        {
            BenchmarkRunner.Run<DependencyBuildBenchmark>();
            BenchmarkRunner.Run<DependencyResolveBenchmark>();
        }

        public static void RunMappers()
        {
            BenchmarkRunner.Run<MappersBenchmark>();
        }

        private static void RunSerialization()
        {
            BenchmarkRunner.Run<DeserializationBenchmark>();
            BenchmarkRunner.Run<SerializationBenchmark>();
        }
    }
}