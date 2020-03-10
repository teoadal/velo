using System;
using FluentAssertions;
using Moq;
using Velo.CQRS.Queries;
using Velo.CQRS.Queries.Pipeline;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Queries
{
    public class QueryPipelineFactoryShould : CQRSTestClass
    {
        private readonly Type _behaviourType;
        private readonly Mock<IDependencyEngine> _engine;
        private readonly QueryPipelineFactory _factory;
        private readonly Mock<IDependency> _processorDependency;
        private readonly Type _pipelineType;
        private readonly Type _preProcessorType;
        private readonly Type _postProcessorType;

        private DependencyLifetime _processorLifetime;

        public QueryPipelineFactoryShould(ITestOutputHelper output) : base(output)
        {
            _factory = new QueryPipelineFactory();
            _pipelineType = typeof(IQueryPipeline<Query, Boo>);

            _behaviourType = typeof(IQueryBehaviour<Query, Boo>);
            _preProcessorType = typeof(IQueryPreProcessor<Query, Boo>);

            _postProcessorType = typeof(IQueryPostProcessor<Query, Boo>);

            _processorDependency = new Mock<IDependency>();
            _processorDependency
                .SetupGet(dependency => dependency.Lifetime)
                .Returns(() => _processorLifetime);

            var processorType = typeof(IQueryProcessor<Query, Boo>);
            _engine = new Mock<IDependencyEngine>();
            _engine
                .Setup(engine => engine.GetDependency(processorType, true))
                .Returns(_processorDependency.Object);
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
            dependency.Resolver.Implementation.Should().Be<QueryFullPipeline<Query, Boo>>();
        }

        [Fact]
        public void CreateFullPipelineWithBehavioursOnly()
        {
            SetupBehaviourProcessors();

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be<QueryFullPipeline<Query, Boo>>();
        }

        [Fact]
        public void CreateSimplePipeline()
        {
            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be<QuerySimplePipeline<Query, Boo>>();
        }

        [Fact]
        public void CreateSequentialPipeline()
        {
            SetupPreProcessors();
            SetupPostProcessors();

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be<QuerySequentialPipeline<Query, Boo>>();
        }

        [Fact]
        public void CreateSequentialPipelineWithPreProcessorsOnly()
        {
            SetupPreProcessors();

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be<QuerySequentialPipeline<Query, Boo>>();
        }

        [Fact]
        public void CreateSequentialPipelineWithPostProcessorsOnly()
        {
            SetupPostProcessors();

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Resolver.Implementation.Should().Be<QuerySequentialPipeline<Query, Boo>>();
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
                .Setup(engine => engine.GetDependency(_behaviourType.MakeArrayType(), true))
                .Returns(_processorDependency.Object);
        }

        private void SetupPreProcessors()
        {
            _engine.Setup(engine => engine.Contains(_preProcessorType)).Returns(true);
            _engine
                .Setup(engine => engine.GetDependency(_preProcessorType.MakeArrayType(), true))
                .Returns(_processorDependency.Object);
        }

        private void SetupPostProcessors()
        {
            _engine.Setup(engine => engine.Contains(_postProcessorType)).Returns(true);
            _engine
                .Setup(engine => engine.GetDependency(_postProcessorType.MakeArrayType(), true))
                .Returns(_processorDependency.Object);
        }
    }
}