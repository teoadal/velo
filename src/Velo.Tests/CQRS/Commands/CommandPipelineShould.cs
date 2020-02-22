using System;
using System.Threading;
using System.Threading.Tasks;
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

        public CommandPipelineShould(ITestOutputHelper output) : base(output)
        {
            var processor = new Mock<ICommandProcessor<Command>>();

            _provider = new DependencyCollection()
                .AddEmitter()
                .AddScoped(ctx => processor.Object)
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
    }
}