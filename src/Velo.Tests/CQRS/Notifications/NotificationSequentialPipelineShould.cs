using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Moq;
using Velo.CQRS.Notifications;
using Velo.CQRS.Notifications.Pipeline;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Notifications
{
    public class NotificationSequentialPipelineShould : CQRSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly Notification _notification;

        private readonly NotificationSequentialPipeline<Notification> _pipeline;
        private readonly Mock<INotificationProcessor<Notification>>[] _processors;

        public NotificationSequentialPipelineShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _notification = new Notification();

            _processors = Many(5, () => BuildProcessor(_notification, _ct));

            _pipeline = new NotificationSequentialPipeline<Notification>(_processors
                .Select(mock => mock.Object)
                .ToArray());
        }

        [Fact]
        public void ProcessNotification()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Publish(_notification, _ct))
                .Should().NotThrow();

            foreach (var processor in _processors)
            {
                processor.Verify(p => p.Process(_notification, _ct));
            }
        }

        [Fact]
        public void ProcessNotificationByInterface()
        {
            var pipeline = (INotificationPipeline) _pipeline;

            pipeline
                .Awaiting(p => p.Publish(_notification, _ct))
                .Should().NotThrow();

            foreach (var processor in _processors)
            {
                processor.Verify(p => p.Process(_notification, _ct));
            }
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _pipeline.Dispose();

            _pipeline
                .Awaiting(pipeline => pipeline.Publish(_notification, _ct))
                .Should().Throw<NullReferenceException>();
        }
    }
}