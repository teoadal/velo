using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Velo.CQRS.Commands;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;

namespace Velo.CQRS.Queries
{
    public class QueryPipelineShould : TestClass
    {
        private readonly DependencyProvider _provider;

        public QueryPipelineShould(ITestOutputHelper output) : base(output)
        {
            var processor = new Mock<IQueryProcessor<Query, Boo>>();

            _provider = new DependencyCollection()
                .AddEmitter()
                .AddScoped(ctx => processor.Object)
                .BuildProvider();
        }

        [Fact]
        public async Task DisposedAfterCloseScope()
        {
            QueryPipeline<Query, Boo> pipeline;
            using (var scope = _provider.CreateScope())
            {
                pipeline = scope.GetRequiredService<QueryPipeline<Query, Boo>>();
            }

            await Assert.ThrowsAsync<NullReferenceException>(
                () => pipeline.GetResponse(It.IsAny<Query>(), CancellationToken.None));
        }
    }
}