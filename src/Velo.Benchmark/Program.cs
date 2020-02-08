using BenchmarkDotNet.Running;

namespace Velo.Benchmark
{
    internal class Program
    {
        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            BenchmarkSwitcher
                .FromAssembly(typeof(Program).Assembly)
                .Run(args);
        }
    }
}