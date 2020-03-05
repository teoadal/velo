using System;
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
    public class QuerySimplePipelineShould : CQRSTestClass
    {
        private readonly CancellationToken _ct;
        private readonly Query _query;
        private readonly Boo _result;
        
        private readonly QuerySimplePipeline<Query, Boo> _pipeline;
        private readonly Mock<IQueryProcessor<Query, Boo>> _processor;
        
        public QuerySimplePipelineShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;
            _query = new Query();
            _result = new Boo();
            
            _processor = new Mock<IQueryProcessor<Query, Boo>>();
            _processor.Setup(processor => processor.Process(_query, _ct))
                .Returns(Task.FromResult(_result));
                
            _pipeline = new QuerySimplePipeline<Query, Boo>(_processor.Object);
        }

        [Fact]
        public async Task GetResponse()
        {
            var response = await _pipeline.GetResponse(_query, _ct);
            response.Should().Be(_result);
        }

        [Fact]
        public async Task GetResponseBuInterface()
        {
            var pipeline = (IQueryPipeline<Boo>) _pipeline;
            var response = await pipeline.GetResponse(_query, _ct);
            response.Should().Be(_result);
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
        public void ThrowIfDisposed()
        {
            _pipeline.Dispose();

            _pipeline
                .Awaiting(pipeline => pipeline.GetResponse(_query, _ct))
                .Should().Throw<NullReferenceException>();
        }
    }
}