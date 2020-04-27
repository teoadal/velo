using System;
using FluentAssertions;
using Moq;
using Velo.CQRS.Commands;
using Velo.CQRS.Commands.Pipeline;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Commands
{
    public class CommandPipelineFactoryShould : CQRSTestClass
    {
        private readonly Type _behaviourType;
        private readonly Mock<IDependencyEngine> _engine;
        private readonly CommandPipelineFactory _factory;
        private readonly Mock<IDependency> _processorDependency;
        private readonly Type _pipelineType;
        private readonly Type _preProcessorType;
        private readonly Type _postProcessorType;

        private DependencyLifetime _processorLifetime;

        public CommandPipelineFactoryShould(ITestOutputHelper output) : base(output)
        {
            _factory = new CommandPipelineFactory();
            
            _pipelineType = typeof(ICommandPipeline<Command>);
            _behaviourType = typeof(ICommandBehaviour<Command>);
            _preProcessorType = typeof(ICommandPreProcessor<Command>);
            _postProcessorType = typeof(ICommandPostProcessor<Command>);

            _processorDependency = new Mock<IDependency>();
            _processorDependency
                .SetupGet(dependency => dependency.Lifetime)
                .Returns(() => _processorLifetime);

            _engine = TestUtils.MockDependencyEngine(typeof(ICommandProcessor<Command>), _processorDependency.Object);
        }

        [Fact]
        public void Applicable()
        {
            _factory.Applicable(_pipelineType).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void CreateWithValidLifetime(DependencyLifetime lifetime)
        {
            _processorLifetime = lifetime;

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);

            dependency.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void CreateFullPipeline()
        {
            SetupBehaviourProcessors();
            SetupPreProcessors();
            SetupPostProcessors();

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be<CommandFullPipeline<Command>>();
        }

        [Fact]
        public void CreateFullPipelineWithBehavioursOnly()
        {
            SetupBehaviourProcessors();

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be<CommandFullPipeline<Command>>();
        }

        [Fact]
        public void CreateSimplePipeline()
        {
            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be<CommandSimplePipeline<Command>>();
        }

        [Fact]
        public void CreateSequentialPipeline()
        {
            SetupPreProcessors();
            SetupPostProcessors();

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be<CommandSequentialPipeline<Command>>();
        }

        [Fact]
        public void CreateSequentialPipelineWithPreProcessorsOnly()
        {
            SetupPreProcessors();

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be<CommandSequentialPipeline<Command>>();
        }

        [Fact]
        public void CreateSequentialPipelineWithPostProcessorsOnly()
        {
            SetupPostProcessors();

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be<CommandSequentialPipeline<Command>>();
        }

        [Fact]
        public void CheckDependencies()
        {
            _factory.BuildDependency(_pipelineType, _engine.Object);

            _engine.Verify(engine => engine.Contains(_behaviourType));
            _engine.Verify(engine => engine.Contains(_postProcessorType));
            _engine.Verify(engine => engine.Contains(_preProcessorType));
        }

        [Fact]
        public void NotApplicable()
        {
            _factory.Applicable(typeof(Boo)).Should().BeFalse();
        }

        private void SetupBehaviourProcessors()
        {
            _engine.Setup(engine => engine.Contains(_behaviourType)).Returns(true);
            _engine
                .Setup(engine => engine.GetRequiredDependency(_behaviourType.MakeArrayType()))
                .Returns(_processorDependency.Object);
        }

        private void SetupPreProcessors()
        {
            _engine.Setup(engine => engine.Contains(_preProcessorType)).Returns(true);
            _engine
                .Setup(engine => engine.GetRequiredDependency(_preProcessorType.MakeArrayType()))
                .Returns(_processorDependency.Object);
        }

        private void SetupPostProcessors()
        {
            _engine.Setup(engine => engine.Contains(_postProcessorType)).Returns(true);
            _engine
                .Setup(engine => engine.GetRequiredDependency(_postProcessorType.MakeArrayType()))
                .Returns(_processorDependency.Object);
        }
    }
}