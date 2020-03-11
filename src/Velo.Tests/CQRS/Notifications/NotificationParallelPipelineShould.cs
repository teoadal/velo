using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Velo.CQRS.Notifications;
using Velo.CQRS.Notifications.Pipeline;
using Velo.TestsModels.Emitting.Parallel;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Notifications
{
    public class NotificationParallelPipelineShould : CQRSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly ParallelNotification _notification;

        private readonly NotificationSequentialPipeline<ParallelNotification> _pipeline;
        private readonly Mock<INotificationProcessor<ParallelNotification>>[] _processors;

        public NotificationParallelPipelineShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _notification = new ParallelNotification();

            _processors = BuildMany(5, BuildProcessor);

            _pipeline = new NotificationSequentialPipeline<ParallelNotification>(_processors
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

        private Mock<INotificationProcessor<ParallelNotification>> BuildProcessor()
        {
            var processor = new Mock<INotificationProcessor<ParallelNotification>>();
            processor
                .Setup(p => p.Process(_notification, _ct))
                .Returns(Task.CompletedTask);

            return processor;
        }
    }
}