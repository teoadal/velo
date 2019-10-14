using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Velo.Collections;

namespace Velo.Benchmark.Collections
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    public class SequenceEnumerationBenchmark
    {
        [Params(15, 10000)]
        public int Count;

        private List<int> _list;
        private Sequence<int> _sequence;

        [GlobalSetup]
        public void Init()
        {
            var items = Enumerable.Range(0, Count).ToArray();

            _list = new List<int>(items);
            _sequence = new Sequence<int>(items);
        }

        [Benchmark(Baseline = true)]
        public int List()
        {
            var counter = 0;
            foreach (var item in _list)
            {
                counter += item;
            }

            return counter;
        }

        [Benchmark]
        public int Sequence()
        {
            var counter = 0;
            foreach (var item in _sequence)
            {
                counter += item;
            }

            return counter;
        }

        [Benchmark]
        public int Sequence_UnderlyingArray()
        {
            var counter = 0;
            var array = _sequence.GetUnderlyingArray(out var length);
            for (var i = 0; i < array.Length; i++)
            {
                if (i == length) break;
                counter += array[i];
            }

            return counter;
        }
    }
}