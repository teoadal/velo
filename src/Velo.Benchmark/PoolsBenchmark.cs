using System.Buffers;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Velo.Pools;

namespace Velo.Benchmark
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class PoolsBenchmark
    {
        [Params(10)] 
        public int Count;

        private ArrayPool<int> _pool;
        private Pool<int[]> _spin;

        [GlobalSetup]
        public void Init()
        {
            static int[] Builder() => new int[10];
            
            _pool = ArrayPool<int>.Create(10, 10);
            _spin = new Pool<int[]>(Count, Builder);
        }

        [BenchmarkCategory("Get")]
        [Benchmark(Baseline = true)]
        public int ArrayPool()
        {
            Parallel.For(0, Count, i =>
            {
                var array = _pool.Rent(10);
                _pool.Return(array);
            });
        
            return _pool.Rent(5).Length;
        }
        
        [BenchmarkCategory("Get")]
        [Benchmark]
        public int Spin()
        {
            Parallel.For(0, Count, i =>
            {
                var array = _spin.Get();
                _spin.Return(array);
            });
        
            return _spin.Get().Length;
        }
    }
}