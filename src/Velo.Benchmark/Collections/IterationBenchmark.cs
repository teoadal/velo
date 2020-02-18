using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using Velo.TestsModels.Boos;

namespace Velo.Benchmark.Collections
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    public class IterationBenchmark
    {
        [Params(10, 15, 25)] 
        public int Count;
        
        private Boo[] _array;
        private List<Boo> _list;

        [GlobalSetup]
        public void Init()
        {
            _array = Enumerable.Range(0, Count).Select(id => new Boo {Id = id, Int = id}).ToArray();
            _list = new List<Boo>(_array);
        }

        [Benchmark(Baseline = true)]
        public int For_Array()
        {
            var sum = 0;
            for (var i = 0; i < _array.Length; i++)
            {
                var element = _array[i];
                sum += element.Id;
            }

            return sum;
        }
        
        [Benchmark]
        public int For_Array_Ref()
        {
            var sum = 0;
            for (var i = 0; i < _array.Length; i++)
            {
                ref readonly var element = ref _array[i];
                sum += element.Id;
            }

            return sum;
        }

        [Benchmark]
        public int For_List()
        {
            var sum = 0;
            for (var i = 0; i < _list.Count; i++)
            {
                var element = _list[i];
                sum += element.Id;
            }

            return sum;
        }
        
        [Benchmark]
        public int For_Span()
        {
            var span = _array.AsSpan();
            
            var sum = 0;
            for (var i = 0; i < span.Length; i++)
            {
                var element = span[i];
                sum += element.Id;
            }

            return sum;
        }
        
        [Benchmark]
        public int For_Span_Ref()
        {
            var span = _array.AsSpan();
            
            var sum = 0;
            for (var i = 0; i < span.Length; i++)
            {
                ref readonly var element = ref span[i];
                sum += element.Id;
            }

            return sum;
        }
        
        [Benchmark]
        public int Foreach_Array()
        {
            var sum = 0;
            foreach (var element in _array)
            {
                sum += element.Id;
            }

            return sum;
        }
        
        [Benchmark]
        public int Foreach_List()
        {
            var sum = 0;
            foreach (var element in _list)
            {
                sum += element.Id;
            }

            return sum;
        }
        
        [Benchmark]
        public int Foreach_Span()
        {
            var span = _array.AsSpan();
            
            var sum = 0;
            foreach (var element in span)
            {
                sum += element.Id;
            }

            return sum;
        }
    }
}