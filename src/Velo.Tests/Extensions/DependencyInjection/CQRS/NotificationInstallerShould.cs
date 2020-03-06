using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Velo.CQRS.Notifications;
using Velo.CQRS.Notifications.Pipeline;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Parallel;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Extensions.DependencyInjection.CQRS
{
    public class NotificationInstallerShould : ServiceCollectionTests
    {
        private readonly INotificationProcessor<Notification> _processor;
        private readonly IServiceCollection _services;

        public NotificationInstallerShould(ITestOutputHelper output) : base(output)
        {
            _processor = BuildProcessor<Notification>();
            _services = new ServiceCollection()
                .AddEmitter();
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddProcessor(ServiceLifetime lifetime)
        {
            _services.AddNotificationProcessor<NotificationProcessor>(lifetime);

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(INotificationProcessor<Notification>) &&
                descriptor.ImplementationType == typeof(NotificationProcessor) &&
                descriptor.Lifetime == lifetime);
        }

        [Fact]
        public void AddProcessorInstance()
        {
            _services.AddNotificationProcessor(_processor);

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(INotificationProcessor<Notification>) &&
                descriptor.ImplementationInstance.IsSameOrEqualTo(_processor) &&
                descriptor.Lifetime == ServiceLifetime.Singleton);
        }

        [Fact]
        public void ResolveParallelPipeline()
        {
            _services
                .AddNotificationProcessor(BuildProcessor<ParallelNotification>())
                .AddNotificationProcessor(BuildProcessor<ParallelNotification>())
                .BuildServiceProvider()
                .GetRequiredService<INotificationPipeline<ParallelNotification>>()
                .Should().BeOfType<NotificationParallelPipeline<ParallelNotification>>();
        }

        [Fact]
        public void ResolveSimplePipeline()
        {
            _services
                .AddNotificationProcessor(_processor)
                .BuildServiceProvider()
                .GetRequiredService<INotificationPipeline<Notification>>()
                .Should().BeOfType<NotificationSimplePipeline<Notification>>();
        }

        [Fact]
        public void ResolveSequentialPipeline()
        {
            _services
                .AddNotificationProcessor(_processor)
                .AddNotificationProcessor(BuildProcessor<Notification>())
                .BuildServiceProvider()
                .GetRequiredService<INotificationPipeline<Notification>>()
                .Should().BeOfType<NotificationSequentialPipeline<Notification>>();
        }

        private static INotificationProcessor<TNotification> BuildProcessor<TNotification>() 
            where TNotification : INotification
        {
            return new Mock<INotificationProcessor<TNotification>>().Object;
        }
    }
}