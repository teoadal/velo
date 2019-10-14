using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Velo.Collections;

namespace Velo.Benchmark.Collections
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    public class SequenceRemoveBenchmark
    {
        [Params(15, 10000)] 
        public int Count;

        private int[] _items;
        private List<int> _list;
        private Sequence<int> _sequence;

        [GlobalSetup]
        public void Init()
        {
            _items = Enumerable.Range(0, Count).ToArray();

            _list = new List<int>(_items);
            _sequence = new Sequence<int>(_items);
        }

        [Benchmark(Baseline = true)]
        public int List()
        {
            var counter = 0;
            for (var i = 0; i < _items.Length; i++)
            {
                if (_list.Remove(_items[i])) counter++;
            }

            return counter;
        }

        [Benchmark]
        public int Sequence()
        {
            var counter = 0;
            for (var i = 0; i < _items.Length; i++)
            {
                if (_sequence.Remove(_items[i])) counter++;
            }

            return counter;
        }
    }
}