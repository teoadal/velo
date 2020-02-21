using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using fastJSON;
using Newtonsoft.Json;
using Velo.Serialization;
using Velo.TestsModels;
using JsonSerializer = SpanJson.JsonSerializer;

namespace Velo.Benchmark.Serialization
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    public class DeserializationBenchmark
    {
        [Params(10000)] 
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
                var instance = SerializationBuilder.CreateBigObject(random);
                _dataset[i] = JsonConvert.SerializeObject(instance);
            }

            _converter = new JConverter();
        }

        [Benchmark(Baseline = true)]
        public long Newtonsoft()
        {
            long stub = 0;

            foreach (var element in _dataset)
            {
                var deserialized = JsonConvert.DeserializeObject<BigObject>(element);
                stub += deserialized.Int;
            }

            return stub;
        }

        [Benchmark]
        public long FastJson()
        {
            long stub = 0;

            foreach (var element in _dataset)
            {
                var deserialized = JSON.ToObject<BigObject>(element);
                stub += deserialized.Int;
            }

            return stub;
        }

        [Benchmark]
        public long SpanJson()
        {
            long stub = 0;

            foreach (var element in _dataset)
            {
                var deserialized = JsonSerializer.Generic.Utf16.Deserialize<BigObject>(element);
                stub += deserialized.Int;
            }

            return stub;
        }

        [Benchmark]
        public long Velo()
        {
            long stub = 0;

            foreach (var element in _dataset)
            {
                var deserialized = _converter.Deserialize<BigObject>(element);
                stub += deserialized.Int;
            }

            return stub;
        }
    }
}