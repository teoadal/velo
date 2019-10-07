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
    public class AskAnonymousTests : TestBase
    {
        private readonly DependencyBuilder _builder;

        public AskAnonymousTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();
        }

        [Theory, AutoData]
        public void Ask_Anonymous(int id)
        {
            var container = _builder
                .AddQueryHandler<GetBoo, Boo>((ctx, payload) => ctx
                    .Resolve<IBooRepository>()
                    .GetElement(payload.Id))
                .BuildContainer();

            var emitter = container.Resolve<Emitter>();

            var repository = container.Resolve<IBooRepository>();
            repository.AddElement(new Boo {Id = id});

            var boo = emitter.Ask(new GetBoo {Id = id});

            Assert.Equal(id, boo.Id);
        }
    }
}