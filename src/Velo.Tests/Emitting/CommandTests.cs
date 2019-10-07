using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class CommandTests : TestBase
    {
        private readonly Emitter _emitter;
        private readonly IBooRepository _repository;

        public CommandTests(ITestOutputHelper output) : base(output)
        {
            var container = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddCommandHandler<CreateBooHandler>()
                .AddCommandHandler<UpdateBooHandler>()
                .AddEmitter()
                .BuildContainer();

            _emitter = container.Resolve<Emitter>();
            _repository = container.Resolve<IBooRepository>();
        }

        [Theory, AutoData]
        public void Execute(int id, bool boolean, int number)
        {
            _emitter.Execute(new CreateBoo {Id = id, Bool = boolean, Int = number});

            var boo = _repository.GetElement(id);

            Assert.Equal(id, boo.Id);
            Assert.Equal(boolean, boo.Bool);
            Assert.Equal(number, boo.Int);
        }

        [Theory, AutoData]
        public async Task Execute_Async(int id, bool boolean, int number)
        {
            _repository.AddElement(new Boo {Id = id});
            
            await _emitter.ExecuteAsync(new UpdateBoo {Id = id, Bool = boolean, Int = number});

            var boo = _repository.GetElement(id);

            Assert.Equal(id, boo.Id);
            Assert.Equal(boolean, boo.Bool);
            Assert.Equal(number, boo.Int);
        }

        [Fact]
        public void Throw_CommandHandler_Not_Registered()
        {
            var bus = new Emitter(new DependencyBuilder().BuildContainer());

            Assert.Throws<KeyNotFoundException>(() => bus.Execute(new CreateBoo()));
        }
    }
}