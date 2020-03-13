using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Velo.CQRS.Queries;
using Velo.CQRS.Queries.Pipeline;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Queries
{
    public class QuerySequentialPipelineShould : CQRSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly Query _query;
        private readonly Boo _result;

        private readonly QuerySequentialPipeline<Query, Boo> _pipeline;

        private readonly Mock<IQueryPreProcessor<Query, Boo>> _preProcessor;
        private readonly Mock<IQueryProcessor<Query, Boo>> _processor;
        private readonly Mock<IQueryPostProcessor<Query, Boo>> _postProcessor;

        public QuerySequentialPipelineShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _query = new Query();
            _result = new Boo();

            _preProcessor = MockQueryPreProcessor<Query, Boo>(_query, _ct);
            _processor = MockQueryProcessor(_query, _result, _ct);
            _postProcessor = MockQueryPostProcessor(_query, _result, _ct);

            _pipeline = new QuerySequentialPipeline<Query, Boo>(
                new[] {_preProcessor.Object},
                _processor.Object,
                new[] {_postProcessor.Object});
        }

        [Fact]
        public async Task GetResponse()
        {
            var response = await _pipeline.GetResponse(_query, _ct);
            response.Should().Be(_result);
        }

        [Fact]
        public async Task GetResponseByInterface()
        {
            var pipeline = (IQueryPipeline<Boo>) _pipeline;
            var response = await pipeline.GetResponse(_query, _ct);
            response.Should().Be(_result);
        }

        [Fact]
        public void PreProcessQuery()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().NotThrow();

            _preProcessor.Verify(processor => processor.PreProcess(_query, _ct));
        }

        [Fact]
        public void ProcessQuery()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().NotThrow();

            _processor.Verify(processor => processor.Process(_query, _ct));
        }

        [Fact]
        public void PostProcessQuery()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().NotThrow();

            _postProcessor.Verify(processor => processor.PostProcess(_query, _result, _ct));
        }

        [Fact]
        public void UseManyPreProcessor()
        {
            var preProcessors = Many(() => MockQueryPreProcessor<Query, Boo>(_query, _ct));
            var pipeline = new QuerySequentialPipeline<Query, Boo>(
                preProcessors.Select(mock => mock.Object).ToArray(),
                _processor.Object,
                new[] {_postProcessor.Object});

            pipeline
                .Awaiting(pipe => pipe.GetResponse(_query, _ct))
                .Should().NotThrow();

            foreach (var preProcessor in preProcessors)
            {
                preProcessor.Verify(processor => processor.PreProcess(_query, _ct));
            }
        }

        [Fact]
        public void UseManyPostProcessor()
        {
            var postProcessors = Many(() => MockQueryPostProcessor(_query, _result, _ct));
            var pipeline = new QuerySequentialPipeline<Query, Boo>(
                new[] {_preProcessor.Object},
                _processor.Object,
                postProcessors.Select(mock => mock.Object).ToArray());

            pipeline
                .Awaiting(pipe => pipe.GetResponse(_query, _ct))
                .Should().NotThrow();

            foreach (var postProcessor in postProcessors)
            {
                postProcessor.Verify(processor => processor.PostProcess(_query, _result, _ct));
            }
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _pipeline.Dispose();

            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().Throw<NullReferenceException>();
        }
    }
}