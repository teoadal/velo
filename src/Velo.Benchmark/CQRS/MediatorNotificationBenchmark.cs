using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Plus;

namespace Velo.Benchmark.CQRS
{
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [MarkdownExporterAttribute.GitHub]
    [MeanColumn, MemoryDiagnoser]
    public class MediatorNotificationBenchmark
    {
        [Params(1000)] 
        public int Count;

        private IMediator _mediator;
        private MediatorBuilder.Notification[] _mediatorNotifications;

        private Emitter _emitter;
        private PlusNotification[] _emitterNotifications;

        [GlobalSetup]
        public void Init()
        {
            _emitterNotifications = new PlusNotification[Count];
            _mediatorNotifications = new MediatorBuilder.Notification[Count];

            for (int i = 0; i < Count; i++)
            {
                _emitterNotifications[i] = new PlusNotification();
                _mediatorNotifications[i] = new MediatorBuilder.Notification();
            }

            var repository = new BooRepository(null, null);

            _mediator = MediatorBuilder.BuildMediatR(repository, services => services
                .AddSingleton<INotificationHandler<MediatorBuilder.Notification>, MediatorBuilder.NotificationHandler>()
                .AddSingleton<INotificationHandler<MediatorBuilder.Notification>, MediatorBuilder.NotificationHandler>()
                .AddSingleton<INotificationHandler<MediatorBuilder.Notification>, MediatorBuilder.NotificationHandler>()
                .AddSingleton<INotificationHandler<MediatorBuilder.Notification>, MediatorBuilder.NotificationHandler>()
                .AddSingleton<INotificationHandler<MediatorBuilder.Notification>, MediatorBuilder.NotificationHandler>());
            
            _emitter = MediatorBuilder.BuildEmitter(repository, services => services
                .AddNotificationProcessor<PlusNotificationProcessor>()
                .AddNotificationProcessor<PlusNotificationProcessor>()
                .AddNotificationProcessor<PlusNotificationProcessor>()
                .AddNotificationProcessor<PlusNotificationProcessor>()
                .AddNotificationProcessor<PlusNotificationProcessor>());
        }

        [Benchmark(Baseline = true)]
        public async Task<long> MediatR()
        {
            var sum = 0L;
            foreach (var notification in _mediatorNotifications)
            {
                await _mediator.Publish(notification);
                sum += notification.Counter;
            }

            return sum;
        }

        [Benchmark]
        public async Task<long> Emitter()
        {
            var sum = 0L;
            foreach (var notification in _emitterNotifications)
            {
                await _emitter.Publish(notification);
                sum += notification.Counter;
            }

            return sum;
        }
    }
}