using AutoMapper;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Velo.Mapping;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;

namespace Velo.Benchmark
{
    [SimpleJob(RuntimeMoniker.NetCoreApp22)]
    [MeanColumn, MemoryDiagnoser]
    public class MappersBenchmark
    {
        [Params(10000, 10003)] 
        public int Count;

        private Boo[] _dataset;

        private IMapper _autoMapper;
        private BasicMapper<Foo> _basicMapper;
        private CompiledMapper<Foo> _compiledMapper;

        [GlobalSetup]
        public void Init()
        {
            _dataset = new Boo[Count];
            for (var i = 0; i < _dataset.Length; i++)
            {
                _dataset[i] = new Boo
                {
                    Bool = i % 10 == 0,
                    Float = i,
                    Int = i,
                    Double = i
                };
            }

            _basicMapper = new BasicMapper<Foo>();

            _compiledMapper = new CompiledMapper<Foo>();
            _compiledMapper.PrepareConverterFor<Boo>();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<Boo, Foo>());
            _autoMapper = config.CreateMapper();
        }

        [Benchmark(Baseline = true)]
        public long AutoMapper()
        {
            long stub = 0;

            for (var i = 0; i < _dataset.Length; i++)
            {
                var element = _dataset[i];
                var mappingResult = _autoMapper.Map<Foo>(element);
                stub += mappingResult.Int;
            }

            return stub;
        }

        [Benchmark]
        public long Velo_BasicMapper()
        {
            long stub = 0;

            for (var i = 0; i < _dataset.Length; i++)
            {
                var element = _dataset[i];
                var mappingResult = _basicMapper.Map(element);
                stub += mappingResult.Int;
            }

            return stub;
        }

        [Benchmark]
        public long Velo_CompiledMapper()
        {
            long stub = 0;

            for (var i = 0; i < _dataset.Length; i++)
            {
                var element = _dataset[i];
                var mappingResult = _compiledMapper.Map(element);
                stub += mappingResult.Int;
            }

            return stub;
        }
    }
}