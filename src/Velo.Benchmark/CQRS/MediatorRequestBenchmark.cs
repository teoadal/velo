using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
using Velo.CQRS.Queries;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Velo.TestsModels.Emitting.PingPong;

namespace Velo.Benchmark.CQRS
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    [CategoriesColumn, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class MediatorRequestBenchmark
    {
        [Params(1000)] 
        public int Count;
        
        private MediatorBuilder.GetBooRequest[] _requests;
        private MediatorBuilder.StructRequest[] _requestsStruct;
        private IMediator _mediator;
        private IMediator _mediatorWithBehaviour;
        private IMediator _mediatorWithFullPipeline;

        private Query[] _queries;
        private Ping[] _queriesStruct;
        private Emitter _emitter;
        private Emitter _emitterWithBehaviour;
        private Emitter _emitterWithFullPipeline;

        [GlobalSetup]
        public void Init()
        {
            var repository = new BooRepository(null, null);

            _queries = new Query[Count];
            _queriesStruct = new Ping[Count];

            _requests = new MediatorBuilder.GetBooRequest[Count];
            _requestsStruct = new MediatorBuilder.StructRequest[Count];

            for (var i = 0; i < Count; i++)
            {
                repository.AddElement(new Boo {Id = i, Int = i});

                _queries[i] = new Query(i);
                _queriesStruct[i] = new Ping(i);

                _requests[i] = new MediatorBuilder.GetBooRequest {Id = i};
                _requestsStruct[i] = new MediatorBuilder.StructRequest(i.ToString());
            }

            _mediator = MediatorBuilder.BuildMediatR(repository);
            _mediatorWithBehaviour = MediatorBuilder.BuildMediatR(repository, services =>
                services.AddSingleton<MediatorBuilder.GetBooRequestBehaviour>());
            _mediatorWithFullPipeline = MediatorBuilder.BuildMediatR(repository, services =>
                services
                    .AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>))
                    .AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>))
                    .AddSingleton<IPipelineBehavior<MediatorBuilder.GetBooRequest, Boo>, MediatorBuilder.GetBooRequestBehaviour>()
                    .AddSingleton<IRequestPreProcessor<MediatorBuilder.GetBooRequest>, MediatorBuilder.GetBooPreProcessor>()
                    .AddSingleton<IRequestPostProcessor<MediatorBuilder.GetBooRequest, Boo>, MediatorBuilder.GetBooPostProcessor>());

            _emitter = _emitterWithBehaviour = MediatorBuilder.BuildEmitter(repository);
            _emitterWithBehaviour = MediatorBuilder.BuildEmitter(repository, services =>
                services.AddSingleton<Behaviour>());
            _emitterWithFullPipeline = MediatorBuilder.BuildEmitter(repository, services =>
                services
                    .AddSingleton<IQueryBehaviour<Query, Boo>, Behaviour>()
                    .AddSingleton<IQueryPreProcessor<Query, Boo>, PreProcessor>()
                    .AddSingleton<IQueryPostProcessor<Query, Boo>, PostProcessor>());
        }

        [BenchmarkCategory("Behaviour")]
        [Benchmark(Baseline = true)]
        public async Task<long> Behaviour_MediatR()
        {
            var sum = 0L;
            foreach (var request in _requests)
            {
                var boo = await _mediatorWithBehaviour.Send(request);
                sum += boo.Int;
            }

            return sum;
        }

        [BenchmarkCategory("Behaviour")]
        [Benchmark]
        public async Task<long> Behaviour_Emitter()
        {
            var sum = 0L;
            foreach (var query in _queries)
            {
                var boo = await _emitterWithBehaviour.Ask(query);
                sum += boo.Int;
            }

            return sum;
        }

        [BenchmarkCategory("Pipeline")]
        [Benchmark(Baseline = true)]
        public async Task<long> FullPipeline_MediatR()
        {
            var sum = 0L;
            foreach (var request in _requests)
            {
                var boo = await _mediatorWithFullPipeline.Send(request);
                sum += boo.Int;
            }

            return sum;
        }

        [BenchmarkCategory("Pipeline")]
        [Benchmark]
        public async Task<long> FullPipeline_Emitter()
        {
            var sum = 0L;
            foreach (var query in _queries)
            {
                var boo = await _emitterWithFullPipeline.Ask(query);
                sum += boo.Int;
            }

            return sum;
        }

        [BenchmarkCategory("Request")]
        [Benchmark(Baseline = true)]
        public async Task<long> Request_MediatR()
        {
            var sum = 0L;
            foreach (var request in _requests)
            {
                var boo = await _mediator.Send(request);
                sum += boo.Int;
            }

            return sum;
        }

        [BenchmarkCategory("Request")]
        [Benchmark]
        public async Task<long> Request_Emitter()
        {
            var sum = 0L;
            foreach (var query in _queries)
            {
                var boo = await _emitter.Ask(query);
                sum += boo.Int;
            }

            return sum;
        }

        [BenchmarkCategory("Request")]
        [Benchmark]
        public async Task<long> Request_EmitterConcrete()
        {
            var sum = 0L;
            foreach (var query in _queries)
            {
                var boo = await _emitter.Ask<Query, Boo>(query);
                sum += boo.Int;
            }

            return sum;
        }

        [BenchmarkCategory("Struct")]
        [Benchmark(Baseline = true)]
        public async Task<long> StructRequest_MediatR()
        {
            var sum = 0L;
            foreach (var structRequest in _requestsStruct)
            {
                var response = await _mediator.Send(structRequest);
                sum += response.Message.Length;
            }

            return sum;
        }

        [BenchmarkCategory("Struct")]
        [Benchmark]
        public async Task<long> StructRequest_Emitter()
        {
            var sum = 0L;
            foreach (var ping in _queriesStruct)
            {
                var pong = await _emitter.Ask<Ping, Pong>(ping);
                sum += pong.Message;
            }

            return sum;
        }
    }
}