using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Parallel;
using Xunit;
using Xunit.Abstractions;

namespace Velo.CQRS.Notifications
{
    public class NotificationPipelineShould : TestClass
    {
        private readonly Emitter _emitter;
        private readonly DependencyProvider _provider;

        public NotificationPipelineShould(ITestOutputHelper output) : base(output)
        {
            var processor = new Mock<INotificationProcessor<Notification>>();

            _provider = new DependencyCollection()
                .AddEmitter()
                .AddScoped(ctx => processor.Object)
                .AddNotificationProcessor<ParallelNotificationProcessor>()
                .AddNotificationProcessor<ParallelNotificationProcessor>()
                .AddNotificationProcessor<ParallelNotificationProcessor>()
                .AddNotificationProcessor<ParallelNotificationProcessor>()
                .AddNotificationProcessor<ParallelNotificationProcessor>()
                .BuildProvider();

            _emitter = _provider.GetRequiredService<Emitter>();
        }

        [Fact]
        public async Task ExecutedParallel()
        {
            var processorsCount = _provider.GetRequiredService<ParallelNotificationProcessor[]>().Length;
            
            var notification = new ParallelNotification();
            await _emitter.Publish(notification);

            notification.ExecutedOn.Distinct().Count().Should().Be(processorsCount);
        }
        
        [Fact]
        public async Task DisposedAfterCloseScope()
        {
            NotificationPipeline<Notification> pipeline;
            using (var scope = _provider.CreateScope())
            {
                pipeline = scope.GetRequiredService<NotificationPipeline<Notification>>();
            }

            await Assert.ThrowsAsync<NullReferenceException>(
                () => pipeline.Publish(It.IsAny<Notification>(), CancellationToken.None));
        }
    }
}