using System;

using BenchmarkDotNet.Attributes;

using Newtonsoft.Json;

using Velo.Serialization;

namespace Velo.Benchmark
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    public class SerializationBenchmark
    {
        [Params(10000, 10003)] 
        public int Count;

        private BigObject[] _dataset;
        private JSerializer _jSerializer;

        [GlobalSetup]
        public void Init()
        {
            _dataset = new BigObject[Count];

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
                _dataset[i] = instance;
            }

            _jSerializer = new JSerializer();
            _jSerializer.PrepareConverterFor<BigObject>();
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
        public long JDeserializer()
        {
            long stub = 0;

            for (var i = 0; i < _dataset.Length; i++)
            {
                var element = _dataset[i];
                var serialized = _jSerializer.Serialize(element);
                stub += serialized.Length;
            }

            return stub;
        }
    }
}