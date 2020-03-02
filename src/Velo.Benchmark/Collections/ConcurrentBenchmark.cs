using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Velo.Collections;

namespace Velo.Benchmark.Collections
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class ConcurrentBenchmark
    {
        private Type _key;
        private Type[] _keys;

        [GlobalSetup]
        public void Init()
        {
            _key = typeof(ConcurrentBenchmark);
            _keys = typeof(LocalList<>).Assembly.DefinedTypes.Select(t => t.AsType()).Take(100).ToArray();
        }

        [BenchmarkCategory("Any")]
        [Benchmark(Baseline = true)]
        public int Concurrent_Any()
        {
            var dic = new ConcurrentDictionary<Type, string>();

            var stub = 0;
            Parallel.For(0, 100, _ =>
            {
                var value = dic.GetOrAdd(_key, key => key.Name);
                if (value.Length % 2 == 0) stub++;
            });

            return stub;
        }

        [BenchmarkCategory("Any")]
        [Benchmark]
        public int Vector_Any()
        {
            var vector = new DangerousVector<Type, string>();

            var stub = 0;
            Parallel.For(0, 100, _ =>
            {
                var value = vector.GetOrAdd(_key, key => key.Name);
                if (value.Length % 2 == 0) stub++;
            });

            return stub;
        }

        [BenchmarkCategory("Many")]
        [Benchmark(Baseline = true)]
        public int Concurrent_Many()
        {
            var dic = new ConcurrentDictionary<Type, string>();

            var stub = 0;
            Parallel.ForEach(_keys, key =>
            {
                var value = dic.GetOrAdd(_key, k => k.Name);
                if (value.Length % 2 == 0) stub++;
            });

            return stub;
        }

        [BenchmarkCategory("Many")]
        [Benchmark]
        public int Vector_Many()
        {
            var vector = new DangerousVector<Type, string>();

            var stub = 0;
            Parallel.ForEach(_keys, key =>
            {
                var value = vector.GetOrAdd(_key, k => k.Name);
                if (value.Length % 2 == 0) stub++;
            });

            return stub;
        }
    }
}