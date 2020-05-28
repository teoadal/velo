using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
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
        [Params(1, 1000)] 
        public int Count;

        private GetBooRequest[] _requests;
        private StructRequest[] _requestsStruct;
        private IMediator _mediator;
        private IMediator _mediatorPipeline;

        private Query[] _queries;
        private Ping[] _queriesStruct;
        private IEmitter _emitter;
        private IEmitter _emitterPipeline;

        [GlobalSetup]
        public void Init()
        {
            var repository = new BooRepository(null);

            _queries = new Query[Count];
            _queriesStruct = new Ping[Count];

            _requests = new GetBooRequest[Count];
            _requestsStruct = new StructRequest[Count];

            for (var i = 0; i < Count; i++)
            {
                repository.AddElement(new Boo {Id = i, Int = i});

                _queries[i] = new Query(i);
                _queriesStruct[i] = new Ping(i);

                _requests[i] = new GetBooRequest {Id = i};
                _requestsStruct[i] = new StructRequest(i.ToString());
            }

            _mediator = MediatorBuilder.BuildMediatR(repository);

            _mediatorPipeline = MediatorBuilder.BuildMediatR(repository, services =>
                services
                    .AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>))
                    .AddSingleton(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>))
                    .AddSingleton<IPipelineBehavior<GetBooRequest, Boo>, GetBooRequestBehaviour>()
                    .AddSingleton<IRequestPreProcessor<GetBooRequest>, GetBooPreProcessor>()
                    .AddSingleton<IRequestPostProcessor<GetBooRequest, Boo>, GetBooPostProcessor>());

            _emitter = MediatorBuilder.BuildEmitter(repository);
            
            _emitterPipeline = MediatorBuilder.BuildEmitter(repository, services =>
                services
                    .AddQueryBehaviour<Behaviour>()
                    .AddQueryProcessor<PreProcessor>()
                    .AddQueryProcessor<PostProcessor>());
        }

        [BenchmarkCategory("Pipeline")]
        [Benchmark(Baseline = true)]
        public async Task<long> FullPipeline_MediatR()
        {
            var stub = 0L;
            foreach (var request in _requests)
            {
                var boo = await _mediatorPipeline.Send(request);
                stub += boo.Int;
            }

            return stub;
        }

        [BenchmarkCategory("Pipeline")]
        [Benchmark]
        public async Task<long> FullPipeline_Emitter()
        {
            var stub = 0L;
            foreach (var query in _queries)
            {
                var boo = await _emitterPipeline.Ask(query);
                stub += boo.Int;
            }

            return stub;
        }

        [BenchmarkCategory("Request")]
        [Benchmark(Baseline = true)]
        public async Task<long> Request_MediatR()
        {
            var stub = 0L;
            foreach (var request in _requests)
            {
                var boo = await _mediator.Send(request);
                stub += boo.Int;
            }

            return stub;
        }

        [BenchmarkCategory("Request")]
        [Benchmark]
        public async Task<long> Request_Emitter()
        {
            var stub = 0L;
            foreach (var query in _queries)
            {
                var boo = await _emitter.Ask(query);
                stub += boo.Int;
            }

            return stub;
        }

        [BenchmarkCategory("Request")]
        [Benchmark]
        public async Task<long> Request_EmitterConcrete()
        {
            var stub = 0L;
            foreach (var query in _queries)
            {
                var boo = await _emitter.Ask<Query, Boo>(query);
                stub += boo.Int;
            }
        
            return stub;
        }
        
        [BenchmarkCategory("Struct")]
        [Benchmark(Baseline = true)]
        public async Task<long> StructRequest_MediatR()
        {
            var stub = 0L;
            foreach (var structRequest in _requestsStruct)
            {
                var response = await _mediator.Send(structRequest);
                stub += response.Message.Length;
            }
        
            return stub;
        }
        
        [BenchmarkCategory("Struct")]
        [Benchmark]
        public async Task<long> StructRequest_Emitter()
        {
            var stub = 0L;
            foreach (var ping in _queriesStruct)
            {
                var pong = await _emitter.Ask<Ping, Pong>(ping);
                stub += pong.Message;
            }
        
            return stub;
        }
    }
}