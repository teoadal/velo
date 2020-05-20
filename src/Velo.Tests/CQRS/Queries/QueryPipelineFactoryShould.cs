using System;
using FluentAssertions;
using Moq;
using Velo.CQRS.Queries;
using Velo.CQRS.Queries.Pipeline;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;

namespace Velo.Tests.CQRS.Queries
{
    public class QueryPipelineFactoryShould : CQRSTestClass
    {
        private readonly Mock<IDependencyEngine> _engine;

        private readonly Type _behaviourType;
        private readonly Type _pipelineType;
        private readonly Type _preProcessorType;
        private readonly Type _postProcessorType;

        private readonly QueryPipelineFactory _factory;

        public QueryPipelineFactoryShould()
        {
            _engine = new Mock<IDependencyEngine>();
            _factory = new QueryPipelineFactory();

            _behaviourType = typeof(IQueryBehaviour<Query, Boo>);
            _pipelineType = typeof(IQueryPipeline<Query, Boo>);
            _preProcessorType = typeof(IQueryPreProcessor<Query, Boo>);
            _postProcessorType = typeof(IQueryPostProcessor<Query, Boo>);
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
            SetupRequiredDependency(_engine, typeof(IQueryProcessor<Query, Boo>), lifetime);

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
            dependency.Implementation.Should().Be<QueryFullPipeline<Query, Boo>>();
        }

        [Fact]
        public void CreateFullPipelineWithBehavioursOnly()
        {
            SetupRequiredDependencies(_engine, _behaviourType);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Implementation.Should().Be<QueryFullPipeline<Query, Boo>>();
        }

        [Fact]
        public void CreateSimplePipeline()
        {
            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Implementation.Should().Be<QuerySimplePipeline<Query, Boo>>();
        }

        [Fact]
        public void CreateSequentialPipeline()
        {
            SetupRequiredDependencies(_engine, _preProcessorType);
            SetupRequiredDependencies(_engine, _postProcessorType);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Implementation.Should().Be<QuerySequentialPipeline<Query, Boo>>();
        }

        [Fact]
        public void CreateSequentialPipelineWithPreProcessorsOnly()
        {
            SetupRequiredDependencies(_engine, _preProcessorType);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Implementation.Should().Be<QuerySequentialPipeline<Query, Boo>>();
        }

        [Fact]
        public void CreateSequentialPipelineWithPostProcessorsOnly()
        {
            SetupRequiredDependencies(_engine, _postProcessorType);

            var dependency = _factory.BuildDependency(_pipelineType, _engine.Object);
            dependency.Implementation.Should().Be<QuerySequentialPipeline<Query, Boo>>();
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