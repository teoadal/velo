using AutoFixture.Xunit2;
using Velo.Dependencies;
using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Boos.Emitting;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Emitting
{
    public class CommandAnonymousTests : TestBase
    {
        private readonly DependencyBuilder _builder;

        public CommandAnonymousTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();
        }

        [Theory, AutoData]
        public void Execute_Anonymous(int id, bool boolean, int number)
        {
            var container = _builder
                .AddCommandHandler<CreateBoo>((ctx, payload) => ctx
                    .Resolve<IBooRepository>()
                    .AddElement(new Boo {Id = payload.Id, Bool = payload.Bool, Int = payload.Int}))
                .BuildContainer();

            var emitter = container.Resolve<Emitter>();
            var repository = container.Resolve<IBooRepository>();

            using (StartStopwatch())
            {
                emitter.Execute(new CreateBoo {Id = id, Bool = boolean, Int = number});    
            }

            var boo = repository.GetElement(id);

            Assert.Equal(id, boo.Id);
            Assert.Equal(boolean, boo.Bool);
            Assert.Equal(number, boo.Int);
        }
    }
}