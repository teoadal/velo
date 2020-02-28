using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
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
        private readonly Mock<IQueryProcessor<Query, Boo>> _processor;

        public QueryPipelineShould(ITestOutputHelper output) : base(output)
        {
            _processor = new Mock<IQueryProcessor<Query, Boo>>();

            _provider = new DependencyCollection()
                .AddEmitter()
                .AddScoped(ctx => _processor.Object)
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

        [Theory]
        [InlineData(DependencyLifetime.Scoped)]
        [InlineData(DependencyLifetime.Singleton)]
        [InlineData(DependencyLifetime.Transient)]
        public void ResolvedByLifetime(DependencyLifetime lifetime)
        {
            var provider = new DependencyCollection()
                .AddEmitter()
                .AddDependency(ctx => _processor.Object, lifetime)
                .BuildProvider();

            var firstScope = provider.CreateScope();
            var firstPipeline = firstScope.GetRequiredService<QueryPipeline<Query, Boo>>();

            var secondScope = provider.CreateScope();
            var secondPipeline = secondScope.GetRequiredService<QueryPipeline<Query, Boo>>();

            if (lifetime == DependencyLifetime.Singleton) firstPipeline.Should().Be(secondPipeline);
            else firstPipeline.Should().NotBe(secondPipeline);
        }
    }
}