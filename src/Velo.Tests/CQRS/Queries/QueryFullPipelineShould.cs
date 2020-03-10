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
    public class QueryFullPipelineShould : CQRSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly Query _query;
        private readonly Boo _result;

        private readonly QueryFullPipeline<Query, Boo> _pipeline;

        private readonly Mock<IQueryBehaviour<Query, Boo>> _behaviour;
        private readonly Mock<IQueryPreProcessor<Query, Boo>> _preProcessor;
        private readonly Mock<IQueryProcessor<Query, Boo>> _processor;
        private readonly Mock<IQueryPostProcessor<Query, Boo>> _postProcessor;

        public QueryFullPipelineShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _query = new Query();
            _result = new Boo();

            _behaviour = BuildBehaviour();

            _preProcessor = BuildPreProcessor();

            _processor = new Mock<IQueryProcessor<Query, Boo>>();
            _processor
                .Setup(processor => processor.Process(_query, _ct))
                .Returns(Task.FromResult(_result));

            _postProcessor = BuildPostProcessor();

            _pipeline = new QueryFullPipeline<Query, Boo>(
                new[] {_behaviour.Object},
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
        public void UseBehaviour()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().NotThrow();

            _behaviour
                .Verify(processor => processor
                    .Execute(_query, It.IsNotNull<Func<Task<Boo>>>(), _ct));
        }

        [Fact]
        public void UseManyBehaviour()
        {
            var behaviours = BuildMany(5, BuildBehaviour);
            var pipeline = new QueryFullPipeline<Query, Boo>(
                behaviours.Select(mock => mock.Object).ToArray(),
                new[] {_preProcessor.Object},
                _processor.Object,
                new[] {_postProcessor.Object});

            pipeline
                .Awaiting(pipe => pipe.GetResponse(_query, _ct))
                .Should().NotThrow();

            foreach (var behaviour in behaviours)
            {
                behaviour
                    .Verify(processor => processor
                        .Execute(_query, It.IsNotNull<Func<Task<Boo>>>(), _ct));
            }
        }

        [Fact]
        public void UseManyPreProcessor()
        {
            var preProcessors = BuildMany(5, BuildPreProcessor);
            var pipeline = new QueryFullPipeline<Query, Boo>(
                new[] {_behaviour.Object},
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
            var postProcessors = BuildMany(5, BuildPostProcessor);
            var pipeline = new QueryFullPipeline<Query, Boo>(
                new[] {_behaviour.Object},
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


        private Mock<IQueryBehaviour<Query, Boo>> BuildBehaviour()
        {
            var behaviour = new Mock<IQueryBehaviour<Query, Boo>>();

            behaviour
                .Setup(processor => processor
                    .Execute(_query, It.IsNotNull<Func<Task<Boo>>>(), _ct))
                .Returns<Query, Func<Task<Boo>>, CancellationToken>((query, next, ct) => next());

            return behaviour;
        }

        private Mock<IQueryPreProcessor<Query, Boo>> BuildPreProcessor()
        {
            var preProcessor = new Mock<IQueryPreProcessor<Query, Boo>>();

            preProcessor
                .Setup(processor => processor.PreProcess(_query, _ct))
                .Returns(Task.CompletedTask);

            return preProcessor;
        }

        private Mock<IQueryPostProcessor<Query, Boo>> BuildPostProcessor()
        {
            var postProcessor = new Mock<IQueryPostProcessor<Query, Boo>>();
            postProcessor
                .Setup(processor => processor.PostProcess(_query, _result, _ct))
                .Returns(Task.CompletedTask);

            return postProcessor;
        }
    }
}