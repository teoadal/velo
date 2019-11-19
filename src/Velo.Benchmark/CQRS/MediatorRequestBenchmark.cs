using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Boos = Velo.TestsModels.Emitting.Boos;

namespace Velo.Benchmark.CQRS
{
    [SimpleJob(RuntimeMoniker.NetCoreApp22)]
    [MeanColumn, MemoryDiagnoser]
    public class MediatorRequestBenchmark
    {
        private const int ElementsCount = 1000;

        private IMediator _mediator;
        private IMediator _mediatorOnVelo;
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
                .AddSingleton<IRequestHandler<GetRequest, Boo>, GetHandler>()
                .AddSingleton<IMediator>(ctx => new Mediator(ctx.GetService))
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            _mediatorOnVelo = new DependencyCollection()
                .AddSingleton<IBooRepository>(ctx => repository)
                .AddSingleton<IRequestHandler<GetRequest, Boo>, GetHandler>()
                .AddSingleton<IMediator>(ctx => new Mediator(ctx.GetService))
                .BuildProvider()
                .GetRequiredService<IMediator>();

            _emitter = new DependencyCollection()
                .AddInstance<IBooRepository>(repository)
                .AddQueryProcessor<Boos.Get.Processor>()
                .AddEmitter()
                .BuildProvider()
                .GetService<Emitter>();
        }

        [Benchmark(Baseline = true)]
        public async Task<long> MediatR()
        {
            long sum = 0;
            for (var i = 0; i < ElementsCount; i++)
            {
                var boo = await _mediator.Send(new GetRequest {Id = i});

                sum += boo.Int;
            }

            return sum;
        }

        [Benchmark]
        public async Task<long> MediatR_OnVelo()
        {
            long sum = 0;
            for (var i = 0; i < ElementsCount; i++)
            {
                var boo = await _mediatorOnVelo.Send(new GetRequest {Id = i});

                sum += boo.Int;
            }

            return sum;
        }

        [Benchmark]
        public async Task<long> Emitter()
        {
            long sum = 0;
            for (var i = 0; i < ElementsCount; i++)
            {
                var boo = await _emitter.Ask(new Boos.Get.Query {Id = i});

                sum += boo.Int;
            }

            return sum;
        }

        private sealed class GetRequest : IRequest<Boo>
        {
            public int Id { get; set; }
        }

        private sealed class GetHandler : IRequestHandler<GetRequest, Boo>
        {
            private readonly IBooRepository _repository;

            public GetHandler(IBooRepository repository)
            {
                _repository = repository;
            }

            public Task<Boo> Handle(GetRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_repository.GetElement(request.Id));
            }
        }
    }
}