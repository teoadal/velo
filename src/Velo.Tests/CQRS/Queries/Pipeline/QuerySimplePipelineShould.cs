using System;
using System.Threading.Tasks;
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
    public class QuerySimplePipelineShould : CQRSTestClass
    {
        private readonly Query _query;
        private readonly Boo _result;
        
        private readonly IQueryPipeline<Query, Boo> _pipeline;
        private readonly Mock<IQueryProcessor<Query, Boo>> _processor;
        
        public QuerySimplePipelineShould()
        {
            _query = new Query();
            _result = new Boo();
            
            _processor = MockQueryProcessor(_query, _result);
            _pipeline = new QuerySimplePipeline<Query, Boo>(_processor.Object);
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
        public void ProcessQuery()
        {
            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, CancellationToken))
                .Should().NotThrow();
            
            EnsurePipelineExecuted();
        }
        
        [Fact]
        public void ThrowIfDisposed()
        {
            _pipeline.Dispose();

            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, CancellationToken))
                .Should().Throw<ObjectDisposedException>();
        }
        
        private void EnsurePipelineExecuted(int count = 1)
        {
            _processor
                .Verify(processor => processor
                    .Process(_query, CancellationToken), Times.Exactly(count));
        }
    }
}