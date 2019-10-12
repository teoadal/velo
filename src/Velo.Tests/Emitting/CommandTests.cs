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

        [Theory, AutoData]
        public async Task Execute_AnonymousAsync(int id, bool boolean, int number)
        {
            var container = _builder
                .AddCommandHandler<CreateBoo>((ctx, payload) => ctx
                    .Resolve<IBooRepository>()
                    .AddElementAsync(new Boo {Id = payload.Id, Bool = payload.Bool, Int = payload.Int}))
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
            var multipleCommandHandler = container.Resolve<MultipleCommandHandler>();
            var emitter = container.Resolve<Emitter>();

            await emitter.ExecuteAsync(new CreateBoo());
            await emitter.ExecuteAsync(new UpdateBoo());

            Assert.True(multipleCommandHandler.CreateBooCalled);
            Assert.True(multipleCommandHandler.UpdateBooCalled);
        }

        [Theory, AutoData]
        public async Task MultipleHandler_With_RegularHandlers(int id, bool boolean, int number, int newNumber)
        {
            var container = _builder
                .AddCommandHandler<MultipleCommandHandler>()
                .AddCommandHandler<CreateBooHandler>()
                .AddCommandHandler<UpdateBooHandler>()
                .BuildContainer();

            var emitter = container.Resolve<Emitter>();
            var multipleCommandHandler = container.Resolve<MultipleCommandHandler>();
            var repository = container.Resolve<IBooRepository>();

            await emitter.ExecuteAsync(new CreateBoo {Id = id, Bool = boolean, Int = number});

            var boo = repository.GetElement(id);

            Assert.NotNull(boo);
            Assert.Equal(id, boo.Id);
            Assert.Equal(boolean, boo.Bool);
            Assert.Equal(number, boo.Int);
            Assert.True(multipleCommandHandler.CreateBooCalled);

            await emitter.ExecuteAsync(new UpdateBoo {Id = id, Int = newNumber});

            Assert.Equal(newNumber, boo.Int);
            Assert.True(multipleCommandHandler.UpdateBooCalled);
        }

        [Fact]
        public async Task PolymorphicHandler()
        {
            var container = _builder.AddCommandHandler<PolymorphicCommandHandler>().BuildContainer();
            var emitter = container.Resolve<Emitter>();
            var polymorphicHandler = container.Resolve<PolymorphicCommandHandler>();

            await emitter.ExecuteAsync(new CreateBoo());
            await emitter.ExecuteAsync(new UpdateBoo());

            Assert.True(polymorphicHandler.CreateBooCalled);
            Assert.True(polymorphicHandler.UpdateBooCalled);
        }

        [Theory, AutoData]
        public async Task PolymorphicHandler_With_RegularHandlers(int id, bool boolean, int number, int newNumber)
        {
            var container = _builder
                .AddCommandHandler<PolymorphicCommandHandler>()
                .AddCommandHandler<CreateBooHandler>()
                .AddCommandHandler<UpdateBooHandler>()
                .BuildContainer();

            var emitter = container.Resolve<Emitter>();
            var polymorphicHandler = container.Resolve<PolymorphicCommandHandler>();
            var repository = container.Resolve<IBooRepository>();

            await emitter.ExecuteAsync(new CreateBoo {Id = id, Bool = boolean, Int = number});

            var boo = repository.GetElement(id);

            Assert.NotNull(boo);
            Assert.Equal(id, boo.Id);
            Assert.Equal(boolean, boo.Bool);
            Assert.Equal(number, boo.Int);
            Assert.True(polymorphicHandler.CreateBooCalled);

            await emitter.ExecuteAsync(new UpdateBoo {Id = id, Int = newNumber});

            Assert.Equal(newNumber, boo.Int);
            Assert.True(polymorphicHandler.UpdateBooCalled);
        }

        [Theory, AutoData]
        public async Task Store(Boo[] boos)
        {
            var container = _builder
                .AddCommandHandler<CreateBooHandler>()
                .BuildContainer();

            var emitter = container.Resolve<Emitter>();
            var repository = container.Resolve<IBooRepository>();
            
            foreach (var boo in boos)
            {
                emitter.Store(new CreateBoo {Id = boo.Id, Bool = boo.Bool, Int = boo.Int});
                Assert.False(repository.Contains(boo.Id));
            }

            WriteLine($"Stored {boos.Length} commands");
            
            using (StartStopwatch())
            {
                await emitter.ProcessStoredAsync();    
            }

            foreach (var boo in boos)
            {
                var actualBoo = repository.GetElement(boo.Id);
                Assert.NotNull(actualBoo);
                Assert.Equal(boo.Id, actualBoo.Id);
                Assert.Equal(boo.Bool, actualBoo.Bool);
                Assert.Equal(boo.Int, actualBoo.Int);
            }
        }
        
        [Fact]
        public async Task Throw_Not_Registered()
        {
            var emitter = new Emitter(new DependencyBuilder().BuildContainer());
            
            await Assert.ThrowsAsync<KeyNotFoundException>(() => emitter.ExecuteAsync(new CreateBoo()));
        }
    }
}