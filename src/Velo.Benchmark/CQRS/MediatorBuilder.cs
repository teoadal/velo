using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
using Velo.Extensions.DependencyInjection.CQRS;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.PingPong;
using Boos = Velo.TestsModels.Emitting.Boos;

namespace Velo.Benchmark.CQRS
{
    public static class MediatorBuilder
    {
        public static IMediator BuildMediatR(IBooRepository repository,
            Action<IServiceCollection> dependencyBuilder = null)
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(repository)
                .AddSingleton<IRequestHandler<GetBooRequest, Boo>, GetBooHandler>()
                .AddSingleton<IRequestHandler<StructRequest, StructResponse>, StructRequestHandler>()
                .AddScoped<IMediator>(ctx => new Mediator(ctx.GetService));

            dependencyBuilder?.Invoke(serviceCollection);

            return serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();
        }

        public static Emitter BuildEmitter(IBooRepository repository,
            Action<IServiceCollection> dependencyBuilder = null)
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton(repository)
                .AddQueryProcessor<PingPongProcessor>()
                .AddQueryProcessor<Boos.Get.Processor>()
                .AddEmitter();

            dependencyBuilder?.Invoke(serviceCollection);

            return serviceCollection
                .BuildServiceProvider()
                .GetService<Emitter>();
        }

        public sealed class Notification : INotification
        {
            public int Counter { get; set; }
        }
        
        public sealed class NotificationHandler: INotificationHandler<Notification>
        {
            public Task Handle(Notification notification, CancellationToken cancellationToken)
            {
                notification.Counter++;
                return Task.CompletedTask;
            }
        }
        
        public sealed class GetBooRequest : IRequest<Boo>
        {
            public int Id { get; set; }

            public bool Measured { get; set; }
            
            public bool PostProcessed { get; set; }

            public bool PreProcessed { get; set; }
        }

        public sealed class GetBooRequestBehaviour : IPipelineBehavior<GetBooRequest, Boo>
        {
            public TimeSpan Elapsed { get; private set; }

            public async Task<Boo> Handle(GetBooRequest request, CancellationToken cancellationToken,
                RequestHandlerDelegate<Boo> next)
            {
                var timer = Stopwatch.StartNew();

                var result = await next();

                Elapsed = timer.Elapsed;
                request.Measured = true;

                return result;
            }
        }

        public sealed class GetBooPreProcessor : IRequestPreProcessor<GetBooRequest>
        {
            public Task Process(GetBooRequest request, CancellationToken cancellationToken)
            {
                request.PreProcessed = true;
                return Task.CompletedTask;
            }
        }

        public sealed class GetBooPostProcessor : IRequestPostProcessor<GetBooRequest, Boo>
        {
            public Task Process(GetBooRequest request, Boo response, CancellationToken cancellationToken)
            {
                request.PostProcessed = true;
                return Task.CompletedTask;
            }
        }

        private sealed class GetBooHandler : IRequestHandler<GetBooRequest, Boo>
        {
            private readonly IBooRepository _repository;

            public GetBooHandler(IBooRepository repository)
            {
                _repository = repository;
            }

            public Task<Boo> Handle(GetBooRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_repository.GetElement(request.Id));
            }
        }

        public readonly struct StructRequest : IRequest<StructResponse>
        {
            public readonly string Message;

            public StructRequest(string message)
            {
                Message = message;
            }
        }

        public readonly struct StructResponse
        {
            public readonly string Message;

            public StructResponse(string message)
            {
                Message = message;
            }
        }

        private sealed class StructRequestHandler : IRequestHandler<StructRequest, StructResponse>
        {
            public Task<StructResponse> Handle(StructRequest request, CancellationToken cancellationToken)
            {
                var response = new StructResponse(request.Message + " Pong");
                return Task.FromResult(response);
            }
        }
    }
}