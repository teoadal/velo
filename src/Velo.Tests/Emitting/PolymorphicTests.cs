using Velo.Dependencies;
using Velo.TestsModels.Boos.Emitting;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Emitting
{
    public class PolymorphicTests : TestBase
    {
        private readonly DependencyContainer _container;
        private readonly Emitter _emitter;

        public PolymorphicTests(ITestOutputHelper output) : base(output)
        {
            _container = new DependencyBuilder()
                .AddCommandHandler<PolymorphicCommandHandler>()
                .AddEmitter()
                .BuildContainer();

            _emitter = _container.Resolve<Emitter>();
        }

        [Fact]
        public void Command()
        {
            _emitter.Execute(new CreateBoo());
            _emitter.Execute(new UpdateBoo());

            var handler = _container.Resolve<PolymorphicCommandHandler>();

            Assert.True(handler.ExecuteWithCreateBooCalled);
            Assert.True(handler.ExecuteWithUpdateBooCalled);
        }
    }
}