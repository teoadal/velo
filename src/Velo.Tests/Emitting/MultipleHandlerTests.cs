using Velo.Dependencies;
using Velo.Emitting.Commands;
using Velo.Emitting.Queries;
using Velo.TestsModels.Boos.Emitting;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Emitting
{
    public class MultipleHandlerTests : TestBase
    {
        public MultipleHandlerTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CommandHandler()
        {
            var container = new DependencyBuilder()
                .AddCommandHandler<MultipleCommandHandler>()
                .AddEmitter()
                .BuildContainer();

            var commandHandler = (MultipleCommandHandler) container.Resolve<ICommandHandler>();
            var emitter = container.Resolve<Emitter>();

            emitter.Execute(new CreateBoo());
            emitter.Execute(new UpdateBoo());

            Assert.True(commandHandler.CreateBooCalled);
            Assert.True(commandHandler.UpdateBooCalled);
        }

        [Fact]
        public void QueryHandler()
        {
            var container = new DependencyBuilder()
                .AddQueryHandler<MultipleQueryHandler>()
                .AddEmitter()
                .BuildContainer();

            var commandHandler = (MultipleQueryHandler) container.Resolve<IQueryHandler>();
            var emitter = container.Resolve<Emitter>();

            emitter.Ask(new GetBoo());
            emitter.Ask(new GetBooInt());

            Assert.True(commandHandler.GetBooCalled);
            Assert.True(commandHandler.GetBooIntCalled);
        }
    }
}