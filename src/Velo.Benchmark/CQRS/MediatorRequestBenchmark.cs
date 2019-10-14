using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Boos.Emitting;

namespace Velo.Benchmark.CQRS
{
    [CoreJob]
    [MeanColumn, MemoryDiagnoser]
    public class MediatorRequestBenchmark
    {
        private const int ElementsCount = 1000;
        
        private IMediator _mediator;
        private Emitter _emitter;

        [GlobalSetup]
        public void Init()
        {
            var repository = new BooRepository(null, null);

            for (var i = 0; i < ElementsCount; i++)
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

            _emitter = new DependencyCollection()
                .AddInstance<IBooRepository>(repository)
                .AddRequestHandler<GetBooHandler>()
                .AddRequestHandler<GetBooIntHandler>()
                .AddMediator()
                .BuildProvider()
                .GetService<Emitter>();
        }

        [Benchmark(Baseline = true)]
        public async Task<long> MediatR()
        {
            long sum = 0;
            for (var i = 0; i < ElementsCount; i++)
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
            for (var i = 0; i < ElementsCount; i++)
            {
                var boo = await _emitter.Send(new GetBoo {Id = i});
                var booInt = await _emitter.Send(new GetBooInt {Id = i});

                sum = sum + boo.Int + booInt;
            }

            return sum;
        }

        private sealed class GetBooRequest : IRequest<Boo>
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

        private sealed class GetBooIntRequest : IRequest<int>
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