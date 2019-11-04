using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Velo.Benchmark.Collections
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    public class ArrayIterationBenchmark
    {
        [Params(21, 10000)] 
        public int Count;

        private int[] _array;
        
        [GlobalSetup]
        public void Init()
        {
            _array = Enumerable.Range(0, Count).ToArray();
        }
        
        [Benchmark(Baseline = true)]
        public int For()
        {
            var counter = 0;
            for (int i = 0; i < _array.Length; i++)
            {
                counter += _array[i];
            }
            
            return counter;
        }
        
        [Benchmark]
        public int Foreach()
        {
            var counter = 0;
            foreach (var number in _array)
            {
                counter += number;
            }
            
            return counter;
        }
    }
}