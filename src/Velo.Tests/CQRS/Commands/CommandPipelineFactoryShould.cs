using System;
using FluentAssertions;
using Moq;
using Velo.CQRS.Commands;
using Velo.CQRS.Commands.Pipeline;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;

namespace Velo.Tests.CQRS.Commands
{
    public class CommandPipelineFactoryShould : CQRSTestClass
    {
        private readonly Mock<IDependencyEngine> _engine;

        private readonly Type _behaviourType;
        private readonly Type _pipelineType;
        private readonly Type _preProcessorType;
        private readonly Type _postProcessorType;

        private readonly CommandPipelineFactory _factory;

        public CommandPipelineFactoryShould()
        {
            _engine = new Mock<IDependencyEngine>();
            _factory = new CommandPipelineFactory();

            _behaviourType = typeof(ICommandBehaviour<Command>);
            _pipelineType = typeof(ICommandPipeline<Command>);
            _preProcessorType = typeof(ICommandPreProcessor<Command>);
            _postProcessorType = typeof(ICommandPostProcessor<Command>);
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
            SetupRequiredDependency(_engine, typeof(ICommandProcessor<Command>), lifetime);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);

            dependency.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void CreateFullPipeline()
        {
            SetupRequiredDependencies(_engine, _behaviourType);
            SetupRequiredDependencies(_engine, _preProcessorType);
            SetupRequiredDependencies(_engine, _postProcessorType);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Implementation.Should().Be<CommandFullPipeline<Command>>();
        }

        [Fact]
        public void CreateFullPipelineWithBehavioursOnly()
        {
            SetupRequiredDependencies(_engine, _behaviourType);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Implementation.Should().Be<CommandFullPipeline<Command>>();
        }

        [Fact]
        public void CreateSimplePipeline()
        {
            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Implementation.Should().Be<CommandSimplePipeline<Command>>();
        }

        [Fact]
        public void CreateSequentialPipeline()
        {
            SetupRequiredDependencies(_engine, _preProcessorType);
            SetupRequiredDependencies(_engine, _postProcessorType);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Implementation.Should().Be<CommandSequentialPipeline<Command>>();
        }

        [Fact]
        public void CreateSequentialPipelineWithPreProcessorsOnly()
        {
            SetupRequiredDependencies(_engine, _preProcessorType);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Implementation.Should().Be<CommandSequentialPipeline<Command>>();
        }

        [Fact]
        public void CreateSequentialPipelineWithPostProcessorsOnly()
        {
            SetupRequiredDependencies(_engine, _postProcessorType);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Implementation.Should().Be<CommandSequentialPipeline<Command>>();
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
    }
}