using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Velo.Dependencies;
using Velo.Emitting;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Boos.Emitting;

namespace Velo.Benchmark.Mediators
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    public class MediatorRequestBenchmark
    {
        [Params(1000)] 
        public int Count;
        
        private IMediator _mediator;
        private Emitter _emitter;
        
        [GlobalSetup]
        public void Init()
        {
            var repository = new BooRepository(null, null);

            for (var i = 0; i < Count; i++)
            {
                repository.AddElement(new Boo {Id = i, Int = i});
            }
            
            _mediator = new ServiceCollection()
                .AddSingleton<IBooRepository>(ctx => repository)
                .AddSingleton<IRequestHandler<GetBooRequest, Boo>, GetBooRequestHandler>()
                .AddSingleton<IRequestHandler<GetBooIntRequest, int>, GetBooIntRequestHandler>()
                .AddSingleton<IMediator>(ctx => new Mediator(ctx.GetService))
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            _emitter = new DependencyBuilder()
                .AddInstance(repository)
                .AddQueryHandler<GetBooHandler>()
                .AddQueryHandler<GetBooIntHandler>()
                .AddEmitter()
                .BuildContainer()
                .Resolve<Emitter>();
        }
        
        [Benchmark(Baseline = true)]
        public async Task<long> MediatR()
        {
            long sum = 0;
            for (var i = 0; i < Count; i++)
            {
                var boo = await _mediator.Send(new GetBooRequest {Id = i});
                var booInt = await _mediator.Send(new GetBooIntRequest {Id = i});

                sum = sum + boo.Int + booInt;
            }

            return sum;
        }

        [Benchmark]
        public async Task<long> Emitter()
        {
            long sum = 0;
            for (var i = 0; i < Count; i++)
            {
                var boo = await _emitter.AskAsync(new GetBoo {Id = i});
                var booInt = await _emitter.AskAsync(new GetBooInt {Id = i});

                sum = sum + boo.Int + booInt;
            }

            return sum;
        }
        
        [Benchmark]
        public async Task<long> Emitter_Concrete()
        {
            long sum = 0;
            for (var i = 0; i < Count; i++)
            {
                var boo = await _emitter.AskAsync<GetBoo, Boo>(new GetBoo {Id = i});
                var booInt = await _emitter.AskAsync<GetBooInt, int>(new GetBooInt {Id = i});

                sum = sum + boo.Int + booInt;
            }

            return sum;
        }
        
        private sealed class GetBooRequest: IRequest<Boo>
        {
            public int Id { get; set; }
        }
        
        private sealed class GetBooRequestHandler : IRequestHandler<GetBooRequest, Boo>
        {
            private readonly IBooRepository _repository;

            public GetBooRequestHandler(IBooRepository repository)
            {
                _repository = repository;
            }

            public Task<Boo> Handle(GetBooRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_repository.GetElement(request.Id));
            }
        }
        
        private sealed class GetBooIntRequest: IRequest<int>
        {
            public int Id { get; set; }
        }
        
        private sealed class GetBooIntRequestHandler : IRequestHandler<GetBooIntRequest, int>
        {
            private readonly IBooRepository _repository;

            public GetBooIntRequestHandler(IBooRepository repository)
            {
                _repository = repository;
            }

            public Task<int> Handle(GetBooIntRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_repository.GetElement(request.Id).Int);
            }
        }
    }
}