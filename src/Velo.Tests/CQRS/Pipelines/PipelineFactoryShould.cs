using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.CQRS.Pipeline;
using Velo.CQRS.Queries;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Pipelines
{
    public class PipelineFactoryShould : TestClass
    {
        private static readonly Type CommandPipelineType = typeof(CommandPipeline<Command>);
        private static readonly Type NotificationPipelineType = typeof(NotificationPipeline<Notification>);
        private static readonly Type QueryPipelineType = typeof(QueryPipeline<Query, Boo>);

        private readonly Mock<IDependencyEngine> _dependencyEngine;
        private DependencyLifetime _processorsLifetime;

        public PipelineFactoryShould(ITestOutputHelper output) : base(output)
        {
            _dependencyEngine = new Mock<IDependencyEngine>();

            _dependencyEngine
                .Setup(e => e.GetDependency(It.IsNotNull<Type>(), true))
                .Returns<Type, bool>((type, _) =>
                {
                    var dependency = new Mock<IDependency>();
                    dependency.SetupGet(d => d.Contracts).Returns(new[] {type});
                    dependency.SetupGet(d => d.Lifetime).Returns(_processorsLifetime);
                    return dependency.Object;
                });
        }

        [Theory]
        [InlineData(DependencyLifetime.Scoped)]
        [InlineData(DependencyLifetime.Singleton)]
        [InlineData(DependencyLifetime.Transient)]
        public void CreateWithValidLifetime(DependencyLifetime lifetime)
        {
            _processorsLifetime = lifetime;

            var factory = new PipelineFactory(PipelineTypes.Command);
            var dependency = factory.BuildDependency(CommandPipelineType, _dependencyEngine.Object);

            dependency.Lifetime.Should().Be(lifetime);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void CreatePipelineDependency(Type pipelineType, Type pipelineGenericType)
        {
            var factory = new PipelineFactory(pipelineGenericType);
            var dependency = factory.BuildDependency(pipelineType, _dependencyEngine.Object);

            dependency.Contracts.Should().Contain(pipelineType);
        }

        [Fact]
        public void CheckCommandPipelineDependencies()
        {
            var factory = new PipelineFactory(PipelineTypes.Command);
            factory.BuildDependency(CommandPipelineType, _dependencyEngine.Object);

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(ICommandBehaviour<Command>[]), true));

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(ICommandPreProcessor<Command>[]), true));

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(ICommandProcessor<Command>), true));

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(ICommandPostProcessor<Command>[]), true));
        }

        [Fact]
        public void CheckNotificationPipelineDependencies()
        {
            var factory = new PipelineFactory(PipelineTypes.Notification);
            factory.BuildDependency(NotificationPipelineType, _dependencyEngine.Object);

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(INotificationProcessor<Notification>[]), true));
        }

        [Fact]
        public void CheckQueryPipelineDependencies()
        {
            var factory = new PipelineFactory(PipelineTypes.Query);
            factory.BuildDependency(QueryPipelineType, _dependencyEngine.Object);

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(IQueryBehaviour<Query, Boo>[]), true));

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(IQueryPreProcessor<Query, Boo>[]), true));

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(IQueryProcessor<Query, Boo>), true));

            _dependencyEngine.Verify(engine => engine
                .GetDependency(typeof(IQueryPostProcessor<Query, Boo>[]), true));
        }

        public static IEnumerable<object[]> Values
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                yield return new object[] { CommandPipelineType, PipelineTypes.Command };
                yield return new object[] { NotificationPipelineType, PipelineTypes.Notification };
                yield return new object[] { QueryPipelineType, PipelineTypes.Query };
            }
        }
    }
}