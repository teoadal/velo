using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Velo.TestsModels.Boos;

namespace Velo.Benchmark.CQRS
{
    internal sealed class Notification : INotification
    {
        public int Counter { get; set; }
    }

    internal sealed class NotificationHandler : INotificationHandler<Notification>
    {
        public Task Handle(Notification notification, CancellationToken cancellationToken)
        {
            notification.Counter++;
            return Task.CompletedTask;
        }
    }

    internal sealed class GetBooRequest : IRequest<Boo>
    {
        public int Id { get; set; }

        public bool Measured { get; set; }

        public bool PostProcessed { get; set; }

        public bool PreProcessed { get; set; }
    }

    internal sealed class GetBooRequestBehaviour : IPipelineBehavior<GetBooRequest, Boo>
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

    internal sealed class GetBooPreProcessor : IRequestPreProcessor<GetBooRequest>
    {
        public Task Process(GetBooRequest request, CancellationToken cancellationToken)
        {
            request.PreProcessed = true;
            return Task.CompletedTask;
        }
    }

    internal sealed class GetBooPostProcessor : IRequestPostProcessor<GetBooRequest, Boo>
    {
        public Task Process(GetBooRequest request, Boo response, CancellationToken cancellationToken)
        {
            request.PostProcessed = true;
            return Task.CompletedTask;
        }
    }

    internal sealed class GetBooHandler : IRequestHandler<GetBooRequest, Boo>
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

    internal readonly struct StructRequest : IRequest<StructResponse>
    {
        public readonly string Message;

        public StructRequest(string message)
        {
            Message = message;
        }
    }

    internal readonly struct StructResponse
    {
        public readonly string Message;

        public StructResponse(string message)
        {
            Message = message;
        }
    }

    internal sealed class StructRequestHandler : IRequestHandler<StructRequest, StructResponse>
    {
        public Task<StructResponse> Handle(StructRequest request, CancellationToken cancellationToken)
        {
            var response = new StructResponse(request.Message + " Pong");
            return Task.FromResult(response);
        }
    }
}