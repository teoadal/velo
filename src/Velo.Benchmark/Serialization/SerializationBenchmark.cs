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
    public class SerializationBenchmark
    {
        [Params(10000)] 
        public int Count;

        private BigObject[] _dataset;
        private JConverter _converter;

        [GlobalSetup]
        public void Init()
        {
            var random = new Random(123);
            _dataset = new BigObject[Count];
            for (var i = 0; i < _dataset.Length; i++)
            {
                _dataset[i] = SerializationBuilder.CreateBigObject(random);
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
                var serialized = JsonConvert.SerializeObject(element);
                stub += serialized.Length;
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
                var serialized = JSON.ToJSON(element);
                stub += serialized.Length;
            }

            return stub;
        }

        [Benchmark]
        public long SpanJson()
        {
            long stub = 0;

            for (var i = 0; i < _dataset.Length; i++)
            {
                var element = _dataset[i];
                var serialized = JsonSerializer.Generic.Utf16.Serialize(element);
                stub += serialized.Length;
            }

            return stub;
        }

        [Benchmark]
        public long Velo()
        {
            long stub = 0;

            for (var i = 0; i < _dataset.Length; i++)
            {
                var element = _dataset[i];
                var serialized = _converter.Serialize(element);
                stub += serialized.Length;
            }

            return stub;
        }
    }
}