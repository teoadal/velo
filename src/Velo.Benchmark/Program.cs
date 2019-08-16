using BenchmarkDotNet.Running;

namespace Velo.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MappersBenchmark>();
        }
    }
}