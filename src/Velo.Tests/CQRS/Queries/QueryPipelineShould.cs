using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Velo.CQRS.Queries;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS.Queries
{
    public class QueryPipelineShould : TestClass
    {
        private readonly Boo _boo;
        private readonly CancellationToken _ct;
        private readonly Query _query;

        private readonly Mock<IQueryBehaviour<Query, Boo>> _behaviour;
        private readonly Mock<IQueryPreProcessor<Query, Boo>> _preProcessor;
        private readonly Mock<IQueryProcessor<Query, Boo>> _processor;
        private readonly Mock<IQueryPostProcessor<Query, Boo>> _postProcessor;

        private readonly QueryPipeline<Query, Boo> _fullPipeline;

        public QueryPipelineShould(ITestOutputHelper output) : base(output)
        {
            _boo = new Boo();
            _ct = CancellationToken.None;
            _query = new Query();

            _behaviour = BuildBehaviour();
            _preProcessor = BuildPreProcessor();
            _postProcessor = BuildPostProcessor();

            _processor = new Mock<IQueryProcessor<Query, Boo>>();
            _processor
                .Setup(processor => processor.Process(_query, _ct))
                .Returns(Task.FromResult(_boo));

            _fullPipeline = new QueryPipeline<Query, Boo>(
                new[] {_behaviour.Object},
                new[] {_preProcessor.Object},
                _processor.Object,
                new[] {_postProcessor.Object});
        }

        [Fact]
        public void ExecuteBehaviour()
        {
            _fullPipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().NotThrow();

            _behaviour
                .Verify(behaviour => behaviour
                    .Execute(_query, It.IsNotNull<Func<Task<Boo>>>(), _ct));
        }

        [Fact]
        public void ExecuteWithoutBehaviour()
        {
            var pipeline = new QueryPipeline<Query, Boo>(
                Array.Empty<IQueryBehaviour<Query, Boo>>(),
                new[] {_preProcessor.Object},
                _processor.Object,
                new[] {_postProcessor.Object});

            pipeline
                .Awaiting(p => p.GetResponse(_query, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void ExecuteWithProcessorAndPostProcessor()
        {
            var pipeline = new QueryPipeline<Query, Boo>(
                Array.Empty<IQueryBehaviour<Query, Boo>>(),
                Array.Empty<IQueryPreProcessor<Query, Boo>>(),
                _processor.Object,
                new[] {_postProcessor.Object});

            pipeline
                .Awaiting(p => p.GetResponse(_query, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void ExecuteWithProcessorAndPreProcessor()
        {
            var pipeline = new QueryPipeline<Query, Boo>(
                Array.Empty<IQueryBehaviour<Query, Boo>>(),
                new[] {_preProcessor.Object},
                _processor.Object,
                Array.Empty<IQueryPostProcessor<Query, Boo>>());

            pipeline
                .Awaiting(p => p.GetResponse(_query, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void ExecuteWithProcessorOnly()
        {
            new QueryPipeline<Query, Boo>(_processor.Object)
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void ExecuteWithManyPreProcessors()
        {
            var preProcessors = Enumerable
                .Range(0, 5)
                .Select(_ => BuildPreProcessor())
                .ToArray();

            var pipeline = new QueryPipeline<Query, Boo>(
                new[] {_behaviour.Object},
                preProcessors.Select(p => p.Object).ToArray(),
                _processor.Object,
                new[] {_postProcessor.Object});

            pipeline
                .Awaiting(p => p.GetResponse(_query, _ct))
                .Should().NotThrow();

            foreach (var preProcessor in preProcessors)
            {
                preProcessor.Verify(processor => processor.PreProcess(_query, _ct));
            }
        }

        [Fact]
        public void ExecuteWithManyPostProcessors()
        {
            var postProcessors = Enumerable
                .Range(0, 5)
                .Select(_ => BuildPostProcessor())
                .ToArray();

            var pipeline = new QueryPipeline<Query, Boo>(
                new[] {_behaviour.Object},
                new[] {_preProcessor.Object},
                _processor.Object,
                postProcessors.Select(p => p.Object).ToArray());

            pipeline
                .Awaiting(p => p.GetResponse(_query, _ct))
                .Should().NotThrow();

            foreach (var postProcessor in postProcessors)
            {
                postProcessor.Verify(processor => processor.PostProcess(_query, _boo, _ct));
            }
        }

        [Fact]
        public void Send()
        {
            var pipeline = (IQueryPipeline<Boo>) _fullPipeline;

            pipeline
                .Awaiting(p => p.GetResponse(_query, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void PreProcess()
        {
            _fullPipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().NotThrow();

            _preProcessor.Verify(processor => processor.PreProcess(_query, _ct));
        }

        [Fact]
        public void Process()
        {
            _fullPipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().NotThrow();

            _processor.Verify(processor => processor.Process(_query, _ct));
        }

        [Fact]
        public void PostProcess()
        {
            _fullPipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().NotThrow();

            _postProcessor.Verify(processor => processor.PostProcess(_query, _boo, _ct));
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _fullPipeline.Dispose();

            _fullPipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().Throw<NullReferenceException>();
        }
        
        [Fact]
        public void ThrowIfDisposedWithoutBehaviour()
        {
            var pipeline = new QueryPipeline<Query, Boo>(_processor.Object);
            
            pipeline.Dispose();

            pipeline
                .Awaiting(p => p.GetResponse(_query, _ct))
                .Should().Throw<NullReferenceException>();
        }
        
        private Mock<IQueryBehaviour<Query, Boo>> BuildBehaviour()
        {
            var behaviour = new Mock<IQueryBehaviour<Query, Boo>>();
            behaviour
                .Setup(b => b.Execute(_query, It.IsNotNull<Func<Task<Boo>>>(), _ct))
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
                .Setup(processor => processor.PostProcess(_query, _boo, _ct))
                .Returns(Task.CompletedTask);

            return postProcessor;
        }
    }
}