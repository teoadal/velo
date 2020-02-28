using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Velo.CQRS;
using Velo.CQRS.Notifications;
using Velo.DependencyInjection;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Parallel;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Notifications
{
    public class NotificationPipelineShould : TestClass
    {
        private readonly IEmitter _emitter;
        private readonly DependencyProvider _provider;
        private readonly Mock<INotificationProcessor<Notification>> _processor;

        public NotificationPipelineShould(ITestOutputHelper output) : base(output)
        {
            _processor = new Mock<INotificationProcessor<Notification>>();

            _provider = new DependencyCollection()
                .AddEmitter()
                .AddScoped(ctx => _processor.Object)
                .AddNotificationProcessor<ParallelNotificationProcessor>()
                .AddNotificationProcessor<ParallelNotificationProcessor>()
                .AddNotificationProcessor<ParallelNotificationProcessor>()
                .AddNotificationProcessor<ParallelNotificationProcessor>()
                .AddNotificationProcessor<ParallelNotificationProcessor>()
                .BuildProvider();

            _emitter = _provider.GetRequiredService<IEmitter>();
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
        
        [Theory]
        [InlineData(DependencyLifetime.Scoped)]
        [InlineData(DependencyLifetime.Singleton)]
        [InlineData(DependencyLifetime.Transient)]
        public void ResolvedByLifetime(DependencyLifetime lifetime)
        {
            var provider = new DependencyCollection()
                .AddEmitter()
                .AddDependency(ctx => _processor.Object, lifetime)
                .BuildProvider();
            
            var firstScope = provider.CreateScope();
            var firstPipeline = firstScope.GetRequiredService<NotificationPipeline<Notification>>();

            var secondScope = provider.CreateScope();
            var secondPipeline = secondScope.GetRequiredService<NotificationPipeline<Notification>>();

            if (lifetime == DependencyLifetime.Singleton) firstPipeline.Should().Be(secondPipeline);
            else firstPipeline.Should().NotBe(secondPipeline);    
        }
    }
}