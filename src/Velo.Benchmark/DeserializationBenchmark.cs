using System;

using BenchmarkDotNet.Attributes;

using fastJSON;

using Newtonsoft.Json;

using Velo.Serialization;
using Velo.TestsModels;

namespace Velo.Benchmark
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    public class DeserializationBenchmark
    {
        [Params(10000, 10003)] 
        public int Count;

        private string[] _dataset;
        private JConverter _converter;

        [GlobalSetup]
        public void Init()
        {
            var random = new Random(123);
            _dataset = new string[Count];
            for (var i = 0; i < _dataset.Length; i++)
            {
                var instance = TestDataBuilder.CreateBigObject(random);
                _dataset[i] = JsonConvert.SerializeObject(instance);
            }

            _converter = new JConverter();
            _converter.PrepareConverterFor<BigObject>();
        }

        [Benchmark(Baseline = true)]
        public long Newtonsoft()
        {
            long stub = 0;

            for (var i = 0; i < _dataset.Length; i++)
            {
                var element = _dataset[i];
                var deserialized = JsonConvert.DeserializeObject<BigObject>(element);
                stub += deserialized.Int;
            }

            return stub;
        }

        [Benchmark]
        public long FastJson()
        {
            long stub = 0;

            for (var i = 0; i < _dataset.Length; i++)
            {
                var element = _dataset[i];
                var deserialized = JSON.ToObject<BigObject>(element);
                stub += deserialized.Int;
            }

            return stub;
        }
        
        [Benchmark]
        public long Velo_Deserializer()
        {
            long stub = 0;

            for (var i = 0; i < _dataset.Length; i++)
            {
                var element = _dataset[i];
                var deserialized = _converter.Deserialize<BigObject>(element);
                stub += deserialized.Int;
            }

            return stub;
        }
    }
}