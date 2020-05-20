using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.CQRS.Queries;
using Velo.CQRS.Queries.Pipeline;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;

namespace Velo.Tests.CQRS.Queries.Pipeline
{
    public class QueryFullPipelineShould : CQRSTestClass
    {
        private readonly Query _query;
        private readonly Boo _result;

        private readonly IQueryPipeline<Query, Boo> _pipeline;

        private readonly Mock<IQueryBehaviour<Query, Boo>> _behaviour;
        private readonly Mock<IQueryPreProcessor<Query, Boo>> _preProcessor;
        private readonly Mock<IQueryProcessor<Query, Boo>> _processor;
        private readonly Mock<IQueryPostProcessor<Query, Boo>> _postProcessor;

        public QueryFullPipelineShould()
        {
            _query = Fixture.Create<Query>();
            _result = Fixture.Create<Boo>();

            _behaviour = MockQueryBehaviour(_query, _result);
            _preProcessor = MockQueryPreProcessor<Query, Boo>(_query);
            _processor = MockQueryProcessor(_query, _result);
            _postProcessor = MockQueryPostProcessor(_query, _result);

            _pipeline = BuildPipeline(new[] {_behaviour}, new[] {_preProcessor}, _processor, new[] {_postProcessor});
        }

        [Fact]
        public void DisposeAfterGetResponse()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, CancellationToken))
                .Should().NotThrow();

            _pipeline
                .Invoking(pipeline => pipeline.Dispose())
                .Should().NotThrow();
        }

        [Fact]
        public void DisposeAgainNotAffect()
        {
            for (var i = 0; i < 5; i++)
            {
                _pipeline
                    .Invoking(pipeline => pipeline.Dispose())
                    .Should().NotThrow();
            }
        }

        [Fact]
        public async Task GetResponse()
        {
            var response = await _pipeline.GetResponse(_query, CancellationToken);
            response.Should().Be(_result);
        }

        [Theory, AutoData]
        public void GetResponseManyTimes(int count)
        {
            count = EnsureValid(count);

            for (var i = 0; i < count; i++)
            {
                _pipeline
                    .Awaiting(pipeline => pipeline.GetResponse(_query, CancellationToken))
                    .Should().NotThrow();
            }

            EnsurePipelineExecuted(count);
        }

        [Theory, AutoData]
        public void GetResponseManyTimesParallel(int count)
        {
            count = EnsureValid(count);

            Parallel.For(0, count, _ =>
            {
                // ReSharper disable once AccessToDisposedClosure
                _pipeline
                    .Awaiting(pipeline => pipeline.GetResponse(_query, CancellationToken))
                    .Should().NotThrow();
            });

            EnsurePipelineExecuted(count);
        }

        [Fact]
        public async Task GetResponseByInterface()
        {
            var pipeline = (IQueryPipeline<Boo>) _pipeline;

            var response = await pipeline.GetResponse(_query, CancellationToken);

            response.Should().Be(_result);
        }

        [Fact]
        public void PreProcessQuery()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, CancellationToken))
                .Should().NotThrow();

            _preProcessor.Verify(processor => processor.PreProcess(_query, CancellationToken));
        }

        [Fact]
        public void ProcessQuery()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, CancellationToken))
                .Should().NotThrow();

            _processor.Verify(processor => processor.Process(_query, CancellationToken));
        }

        [Fact]
        public void PostProcessQuery()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, CancellationToken))
                .Should().NotThrow();

            _postProcessor.Verify(processor => processor.PostProcess(_query, _result, CancellationToken));
        }

        [Fact]
        public void UseBehaviour()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, CancellationToken))
                .Should().NotThrow();

            _behaviour
                .Verify(processor => processor
                    .Execute(_query, It.IsNotNull<Func<Task<Boo>>>(), CancellationToken));
        }

        [Fact]
        public void UseManyBehaviour()
        {
            var behaviours = Many(() => MockQueryBehaviour(_query, _result));

            var pipeline = BuildPipeline(behaviours);

            pipeline
                .Awaiting(pipe => pipe.GetResponse(_query, CancellationToken))
                .Should().NotThrow();

            foreach (var behaviour in behaviours)
            {
                behaviour
                    .Verify(processor => processor
                        .Execute(_query, It.IsNotNull<Func<Task<Boo>>>(), CancellationToken));
            }
        }

        [Fact]
        public void UseManyPreProcessor()
        {
            var preProcessors = Many(() => MockQueryPreProcessor<Query, Boo>(_query));
            var pipeline = BuildPipeline(preProcessors: preProcessors);

            pipeline
                .Awaiting(pipe => pipe.GetResponse(_query, CancellationToken))
                .Should().NotThrow();

            foreach (var preProcessor in preProcessors)
            {
                preProcessor.Verify(processor => processor.PreProcess(_query, CancellationToken));
            }
        }

        [Fact]
        public void UseManyPostProcessor()
        {
            var postProcessors = Many(() => MockQueryPostProcessor(_query, _result));
            var pipeline = BuildPipeline(postProcessors: postProcessors);

            pipeline
                .Awaiting(pipe => pipe.GetResponse(_query, CancellationToken))
                .Should().NotThrow();

            foreach (var postProcessor in postProcessors)
            {
                postProcessor.Verify(processor => processor.PostProcess(_query, _result, CancellationToken));
            }
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _pipeline.Dispose();

            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, CancellationToken))
                .Should().Throw<ObjectDisposedException>();
        }

        private QueryFullPipeline<Query, Boo> BuildPipeline(
            IEnumerable<Mock<IQueryBehaviour<Query, Boo>>> behaviours = null,
            IEnumerable<Mock<IQueryPreProcessor<Query, Boo>>> preProcessors = null,
            Mock<IQueryProcessor<Query, Boo>> processor = null,
            IEnumerable<Mock<IQueryPostProcessor<Query, Boo>>> postProcessors = null)
        {
            return new QueryFullPipeline<Query, Boo>(
                behaviours == null
                    ? new[] {_behaviour.Object}
                    : behaviours.Select(b => b.Object).ToArray(),
                preProcessors == null
                    ? new[] {_preProcessor.Object}
                    : preProcessors.Select(p => p.Object).ToArray(),
                processor?.Object ?? _processor.Object,
                postProcessors == null
                    ? new[] {_postProcessor.Object}
                    : postProcessors.Select(p => p.Object).ToArray());
        }

        private void EnsurePipelineExecuted(int count = 1)
        {
            _behaviour
                .Verify(behaviour => behaviour
                    .Execute(_query, It.IsNotNull<Func<Task<Boo>>>(), CancellationToken), Times.Exactly(count));

            _preProcessor
                .Verify(processor => processor
                    .PreProcess(_query, CancellationToken), Times.Exactly(count));

            _processor
                .Verify(processor => processor
                    .Process(_query, CancellationToken), Times.Exactly(count));

            _postProcessor
                .Verify(processor => processor
                    .PostProcess(_query, _result, CancellationToken), Times.Exactly(count));
        }
    }
}