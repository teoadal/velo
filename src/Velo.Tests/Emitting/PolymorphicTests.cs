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
    public class PolymorphicTests : TestBase
    {
        private readonly DependencyContainer _container;
        private readonly Emitter _emitter;
        private readonly IBooRepository _repository;

        public PolymorphicTests(ITestOutputHelper output) : base(output)
        {
            _container = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddCommandHandler<PolymorphicCommandHandler>()
                .AddEmitter()
                .BuildContainer();

            _emitter = _container.Resolve<Emitter>();
            _repository = _container.Resolve<IBooRepository>();
        }

        [Theory, AutoData]
        public void Command(int id, int number, int updatedNumber)
        {
            _emitter.Execute(new CreateBoo {Id = id, Int = number});

            var boo = _repository.GetElement(id);
            Assert.Equal(number, boo.Int);

            _emitter.Execute(new UpdateBoo {Id = id, Int = updatedNumber});
            Assert.Equal(updatedNumber, boo.Int);
        }
    }
}