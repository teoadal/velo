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
        private readonly DependencyBuilder _builder;

        public CommandTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();
        }

        [Theory, AutoData]
        public async Task Execute(int id, bool boolean, int number)
        {
            var container = _builder.AddCommandHandler<CreateBooHandler>().BuildContainer();
            var emitter = container.Resolve<Emitter>();
            var repository = container.Resolve<IBooRepository>();
            
            await emitter.ExecuteAsync(new CreateBoo {Id = id, Bool = boolean, Int = number});
            
            var boo = repository.GetElement(id);

            Assert.Equal(id, boo.Id);
            Assert.Equal(boolean, boo.Bool);
            Assert.Equal(number, boo.Int);
        }

        [Theory, AutoData]
        public async Task Execute_Anonymous(int id, bool boolean, int number)
        {
            var container = _builder
                .AddCommandHandler<CreateBoo>((ctx, payload) => ctx
                    .Resolve<IBooRepository>()
                    .AddElement(new Boo {Id = payload.Id, Bool = payload.Bool, Int = payload.Int}))
                .BuildContainer();

            var emitter = container.Resolve<Emitter>();
            var repository = container.Resolve<IBooRepository>();

            await emitter.ExecuteAsync(new CreateBoo {Id = id, Bool = boolean, Int = number});

            var boo = repository.GetElement(id);

            Assert.Equal(id, boo.Id);
            Assert.Equal(boolean, boo.Bool);
            Assert.Equal(number, boo.Int);
        }
        
        [Fact]
        public async Task MultipleHandler()
        {
            var container = _builder.AddCommandHandler<MultipleCommandHandler>().BuildContainer();
            var commandHandler = container.Resolve<MultipleCommandHandler>();
            var emitter = container.Resolve<Emitter>();

            await emitter.ExecuteAsync(new CreateBoo());
            await emitter.ExecuteAsync(new UpdateBoo());

            Assert.True(commandHandler.CreateBooCalled);
            Assert.True(commandHandler.UpdateBooCalled);
        }
        
        [Fact]
        public async Task PolymorphicHandler()
        {
            var container = _builder.AddCommandHandler<PolymorphicCommandHandler>().BuildContainer();
            var emitter = container.Resolve<Emitter>();
            
            await emitter.ExecuteAsync(new CreateBoo());
            await emitter.ExecuteAsync(new UpdateBoo());

            var handler = container.Resolve<PolymorphicCommandHandler>();
            Assert.True(handler.ExecuteWithCreateBooCalled);
            Assert.True(handler.ExecuteWithUpdateBooCalled);
        }
        
        [Fact]
        public void Throw_CommandHandler_Not_Registered()
        {
            var emitter = new Emitter(new DependencyBuilder().BuildContainer());
            Assert.ThrowsAsync<KeyNotFoundException>(() => emitter.ExecuteAsync(new CreateBoo()));
        }
    }
}