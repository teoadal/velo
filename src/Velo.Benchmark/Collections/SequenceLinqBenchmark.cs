using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Velo.Collections;
using Velo.TestsModels.Boos;

namespace Velo.Benchmark.Collections
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class SequenceLinqBenchmark
    {
        [Params(15, 10000)]
        public int Count;

        private List<Boo> _list;
        private Sequence<Boo> _sequence;

        [GlobalSetup]
        public void Init()
        {
            var items = Enumerable.Range(0, Count)
                .Select(i => new Boo { Id = i })
                .ToArray();

            _list = new List<Boo>(items);
            _sequence = new Sequence<Boo>(items);
        }

        [BenchmarkCategory("Any"), Benchmark(Baseline = true)]
        public bool Any()
        {
            return _list.Any(e => e.Id % 5 == 0);
        }

        [BenchmarkCategory("Any"), Benchmark]
        public bool Exists()
        {
            return _list.Exists(e => e.Id % 5 == 0);
        }
        
        [BenchmarkCategory("Any"), Benchmark]
        public bool SequenceAny()
        {
            return _sequence.Any(e => e.Id % 5 == 0);
        }

        [BenchmarkCategory("FirstOrDefault"), Benchmark(Baseline = true)]
        public Boo FirstOrDefault()
        {
            return _list.FirstOrDefault(e => e.Id % 5 == 0);
        }

        [BenchmarkCategory("FirstOrDefault"), Benchmark]
        public Boo Find()
        {
            return _list.Find(e => e.Id % 5 == 0);
        }
        
        [BenchmarkCategory("FirstOrDefault"), Benchmark]
        public Boo SequenceFirstOrDefault()
        {
            return _sequence.FirstOrDefault(e => e.Id % 5 == 0);
        }
        
        [BenchmarkCategory("Select"), Benchmark(Baseline = true)]
        public int Select()
        {
            var counter = 0;
            foreach (var id in _list.Select(b => b.Id))
            {
                counter += id;
            }

            return counter;
        }

        [BenchmarkCategory("Select"), Benchmark]
        public int ConvertAll()
        {
            var counter = 0;
            foreach (var id in _list.ConvertAll(b => b.Id))
            {
                counter += id;
            }

            return counter;
        }

        [BenchmarkCategory("Select"), Benchmark]
        public int SequenceSelect()
        {
            var counter = 0;
            foreach (var id in _sequence.Select(b => b.Id))
            {
                counter += id;
            }

            return counter;
        }
        
        [BenchmarkCategory("SelectToArray"), Benchmark(Baseline = true)]
        public int SelectToArray()
        {
            return _list.Select(b => b.Id).ToArray().Length;
        }

        [BenchmarkCategory("SelectToArray"), Benchmark]
        public int SequenceSelectToArray()
        {
            return _sequence.Select(b => b.Id).ToArray().Length;
        }
        
        [BenchmarkCategory("Where"), Benchmark(Baseline = true)]
        public int Where()
        {
            var counter = 0;
            foreach (var boo in _list.Where(b => b.Id % 5 == 0))
            {
                counter += boo.Id;
            }
            
            return counter;
        }

        [BenchmarkCategory("Where"), Benchmark]
        public int FindAll()
        {
            var counter = 0;
            foreach (var boo in _list.FindAll(b => b.Id % 5 == 0))
            {
                counter += boo.Id;
            }
            
            return counter;
        }
        
        [BenchmarkCategory("Where"), Benchmark]
        public int SequenceWhere()
        {
            var counter = 0;
            foreach (var boo in _sequence.Where(b => b.Id % 5 == 0))
            {
                counter += boo.Id;
            }
            
            return counter;
        }
    }
}