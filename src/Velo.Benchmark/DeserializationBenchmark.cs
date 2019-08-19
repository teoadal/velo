using System;

using BenchmarkDotNet.Attributes;

using Newtonsoft.Json;

using Velo.Serialization;

namespace Velo.Benchmark
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    public class DeserializationBenchmark
    {
        [Params(10000, 10003)] public int Count;

        private string[] _dataset;
        private JSerializer _jSerializer;

        [GlobalSetup]
        public void Init()
        {
            _dataset = new string[Count];

            for (var i = 0; i < _dataset.Length; i++)
            {
                var instance = new BigObject
                {
                    Array = new[] {i + 1, i + 2, i + 3},
                    Bool = i % 10 == 0,
                    Boo = new Boo
                    {
                        Bool = i % 5 == 0,
                        Double = i + 1,
                        Float = i + 2,
                        Int = i + 3,
                    },
                    Foo = new Foo
                    {
                        Bool = i % 100 == 0,
                        Float = i + 4,
                        Int = i + 5,
                    },
                    Float = i + 6,
                    Int = i + 7,
                    Double = i + 8,
                    String = Guid.NewGuid().ToString()
                };
                _dataset[i] = JsonConvert.SerializeObject(instance);
            }

            _jSerializer = new JSerializer();
            _jSerializer.PrepareForSource<BigObject>();
        }

        [Benchmark]
        public long JSerializer()
        {
            long stub = 0;

            for (var i = 0; i < _dataset.Length; i++)
            {
                var element = _dataset[i];
                var deserialized = _jSerializer.Deserialize<BigObject>(element);
                stub += deserialized.Int;
            }

            return stub;
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
    }
}