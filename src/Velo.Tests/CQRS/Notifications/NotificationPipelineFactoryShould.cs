using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Velo.CQRS.Notifications;
using Velo.CQRS.Notifications.Pipeline;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Parallel;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Notifications
{
    public class NotificationPipelineFactoryShould : CQRSTestClass
    {
        private readonly Mock<IDependencyEngine> _dependencyEngine;
        private readonly NotificationPipelineFactory _factory;
        private readonly Type _pipelineType;

        private int _processorsCount;
        private DependencyLifetime _processorsLifetime;

        public NotificationPipelineFactoryShould(ITestOutputHelper output) : base(output)
        {
            _dependencyEngine = new Mock<IDependencyEngine>();
            _dependencyEngine
                .Setup(e => e.GetApplicable(It.IsNotNull<Type>()))
                .Returns<Type>(ProcessorsDependencyBuilder);

            _factory = new NotificationPipelineFactory();
            _pipelineType = typeof(INotificationPipeline<Notification>);

            _processorsCount = 1;
            _processorsLifetime = DependencyLifetime.Scoped;
        }

        [Fact]
        public void Applicable()
        {
            _factory.Applicable(_pipelineType).Should().BeTrue();
        }
        
        [Fact]
        public void CreateDependency()
        {
            var dependency = _factory.BuildDependency(_pipelineType, _dependencyEngine.Object);

            dependency.Contracts.Should().Contain(_pipelineType);
            dependency.Resolver.Implementation.Should().Implement(_pipelineType);
        }

        [Fact]
        public void CreateParallelPipeline()
        {
            _processorsCount = 5;

            var pipelineType = typeof(INotificationPipeline<ParallelNotification>);
            var dependency = _factory.BuildDependency(pipelineType, _dependencyEngine.Object);
            
            dependency.Resolver.Implementation.Should().Be<NotificationParallelPipeline<ParallelNotification>>();
        }
        
        [Fact]
        public void CreateSequentialPipeline()
        {
            _processorsCount = 5;
            
            var dependency = _factory.BuildDependency(_pipelineType, _dependencyEngine.Object);
            
            dependency.Resolver.Implementation.Should().Be<NotificationSequentialPipeline<Notification>>();
        }
        
        [Fact]
        public void CreateSimplePipeline()
        {
            _processorsCount = 1;

            var dependency = _factory.BuildDependency(_pipelineType, _dependencyEngine.Object);
            
            dependency.Resolver.Implementation.Should().Be<NotificationSimplePipeline<Notification>>();
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void CreateWithValidLifetime(DependencyLifetime lifetime)
        {
            _processorsLifetime = lifetime;

            var dependency = _factory.BuildDependency(_pipelineType, _dependencyEngine.Object);

            dependency.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void CheckProcessorsCount()
        {
            _factory.BuildDependency(_pipelineType, _dependencyEngine.Object);

            _dependencyEngine
                .Verify(engine => engine
                    .GetApplicable(typeof(INotificationProcessor<Notification>)));
        }

        [Fact]
        public void NotApplicable()
        {
            _factory.Applicable(typeof(Boo)).Should().BeFalse();
        }
        
        private IDependency[] ProcessorsDependencyBuilder(Type processorType)
        {
            return Many(_processorsCount, () => TestUtils.MockDependency(_processorsLifetime, processorType))
                .Select(d => d.Object)
                .ToArray();
        }
    }
}