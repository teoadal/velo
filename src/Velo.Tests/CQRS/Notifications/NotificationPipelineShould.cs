using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Velo.CQRS.Notifications;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Parallel;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Notifications
{
    public class NotificationPipelineShould : TestClass
    {
        private readonly CancellationToken _ct;
        private readonly Notification _notification;

        private readonly Mock<INotificationProcessor<Notification>> _processor;
        private readonly NotificationPipeline<Notification> _pipeline;

        public NotificationPipelineShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _notification = new Notification();

            _processor = BuildProcessor(_notification);

            _pipeline = new NotificationPipeline<Notification>(_processor.Object);
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
        public void ProcessNotificationWithMultipleProcessors()
        {
            var processors = Enumerable
                .Range(0, 5)
                .Select(_ => BuildProcessor(_notification))
                .ToArray();

            new NotificationPipeline<Notification>(processors.Select(p => p.Object).ToArray())
                .Awaiting(pipeline => pipeline.Publish(_notification, _ct))
                .Should().NotThrow();

            foreach (var processor in processors)
            {
                processor.Verify(p => p.Process(_notification, _ct));
            }
        }

        [Fact]
        public void ExecutedParallel()
        {
            var parallelNotification = new ParallelNotification();
            var processors = Enumerable
                .Range(0, 5)
                .Select(_ => BuildProcessor(parallelNotification))
                .ToArray();

            new NotificationPipeline<ParallelNotification>(processors.Select(p => p.Object).ToArray())
                .Awaiting(pipeline => pipeline.Publish(parallelNotification, _ct))
                .Should().NotThrow();

            foreach (var processor in processors)
            {
                processor.Verify(p => p.Process(parallelNotification, _ct));
            }
        }

        [Fact]
        public void Send()
        {
            var pipeline = (INotificationPipeline) _pipeline;

            pipeline
                .Awaiting(p => p.Publish(_notification, _ct))
                .Should().NotThrow();

            _processor.Verify(processor => processor.Process(_notification, _ct));
        }

        [Fact]
        public void StopPropagation()
        {
            _notification.StopPropagation = true;

            _pipeline
                .Awaiting(pipeline => pipeline.Publish(_notification, _ct))
                .Should().NotThrow();

            _processor.Verify(processor => processor.Process(_notification, _ct), Times.Never);
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _pipeline.Dispose();

            _pipeline
                .Awaiting(pipeline => pipeline.Publish(_notification, _ct))
                .Should().Throw<NullReferenceException>();
        }

        private Mock<INotificationProcessor<T>> BuildProcessor<T>(T notification) where T : INotification
        {
            var processor = new Mock<INotificationProcessor<T>>();

            processor
                .Setup(p => p.Process(notification, _ct))
                .Returns(Task.CompletedTask);

            return processor;
        }
    }
}