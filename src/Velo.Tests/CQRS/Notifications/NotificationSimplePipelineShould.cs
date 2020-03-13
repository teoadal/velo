using System;
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
    public class NotificationSimplePipelineShould : CQRSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly Notification _notification;

        private readonly NotificationSimplePipeline<Notification> _pipeline;
        private readonly Mock<INotificationProcessor<Notification>> _processor;

        public NotificationSimplePipelineShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _notification = new Notification();

            _processor = MockNotificationProcessor(_notification, _ct);

            _pipeline = new NotificationSimplePipeline<Notification>(_processor.Object);
        }

        [Fact]
        public void ProcessNotification()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.Publish(_notification, _ct))
                .Should().NotThrow();

            _processor.Verify(processor => processor.Process(_notification, _ct));
        }

        [Fact]
        public void ProcessNotificationByInterface()
        {
            var pipeline = (INotificationPipeline) _pipeline;
            
            pipeline
                .Awaiting(p => p.Publish(_notification, _ct))
                .Should().NotThrow();

            _processor.Verify(processor => processor.Process(_notification, _ct));
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