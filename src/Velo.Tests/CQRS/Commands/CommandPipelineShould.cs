using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;
using Xunit.Abstractions;

namespace Velo.CQRS.Commands
{
    public class CommandPipelineShould : TestClass
    {
        private readonly DependencyProvider _provider;
        private readonly Mock<ICommandProcessor<Command>> _processor;

        public CommandPipelineShould(ITestOutputHelper output) : base(output)
        {
            _processor = new Mock<ICommandProcessor<Command>>();

            _provider = new DependencyCollection()
                .AddEmitter()
                .AddScoped(ctx => _processor.Object)
                .BuildProvider();
        }

        [Fact]
        public async Task DisposedAfterCloseScope()
        {
            CommandPipeline<Command> pipeline;
            using (var scope = _provider.CreateScope())
            {
                pipeline = scope.GetRequiredService<CommandPipeline<Command>>();
            }

            await Assert.ThrowsAsync<NullReferenceException>(
                () => pipeline.Execute(It.IsAny<Command>(), CancellationToken.None));
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
            var firstPipeline = firstScope.GetRequiredService<CommandPipeline<Command>>();

            var secondScope = provider.CreateScope();
            var secondPipeline = secondScope.GetRequiredService<CommandPipeline<Command>>();

            if (lifetime == DependencyLifetime.Singleton) firstPipeline.Should().Be(secondPipeline);
            else firstPipeline.Should().NotBe(secondPipeline);
        }
    }
}