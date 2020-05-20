using System;
using FluentAssertions;
using Moq;
using Velo.CQRS.Notifications;
using Velo.CQRS.Notifications.Pipeline;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Parallel;
using Xunit;

namespace Velo.Tests.CQRS.Notifications
{
    public class NotificationPipelineFactoryShould : CQRSTestClass
    {
        private readonly Mock<IDependencyEngine> _engine;

        private readonly Type _pipelineType;
        private readonly Type _processorType;

        private readonly NotificationPipelineFactory _factory;

        public NotificationPipelineFactoryShould()
        {
            _engine = new Mock<IDependencyEngine>();
            _factory = new NotificationPipelineFactory();

            _pipelineType = typeof(INotificationPipeline<Notification>);
            _processorType = typeof(INotificationProcessor<Notification>);
        }

        [Fact]
        public void Applicable()
        {
            _factory.Applicable(_pipelineType).Should().BeTrue();
        }

        [Fact]
        public void CreateParallelPipeline()
        {
            SetupApplicableDependencies(_engine, typeof(INotificationProcessor<ParallelNotification>));

            var pipelineType = typeof(INotificationPipeline<ParallelNotification>);
            var dependency = _factory.BuildDependency(pipelineType, _engine.Object);
            
            dependency.Implementation.Should().Be<NotificationParallelPipeline<ParallelNotification>>();
        }
        
        [Fact]
        public void CreateSequentialPipeline()
        {
            SetupApplicableDependencies(_engine, _processorType, count: 10);
            
            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            
            dependency.Implementation.Should().Be<NotificationSequentialPipeline<Notification>>();
        }
        
        [Fact]
        public void CreateSimplePipeline()
        {
            SetupApplicableDependencies(_engine, _processorType, count: 1);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            
            dependency.Implementation.Should().Be<NotificationSimplePipeline<Notification>>();
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void CreateWithValidLifetime(DependencyLifetime lifetime)
        {
            SetupApplicableDependencies(_engine, _processorType, lifetime);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);

            dependency.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void CheckProcessorsCount()
        {
            SetupApplicableDependencies(_engine, _processorType);
            
            _factory.BuildDependency(_pipelineType, _engine.Object);

            _engine
                .Verify(engine => engine
                    .GetApplicable(typeof(INotificationProcessor<Notification>)));
        }

        [Fact]
        public void NotApplicable()
        {
            _factory.Applicable(typeof(Boo)).Should().BeFalse();
        }
    }
}