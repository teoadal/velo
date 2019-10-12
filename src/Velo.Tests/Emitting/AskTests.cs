using System.Collections.Generic;
using System.Reflection;
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
    public class AskTests : TestBase
    {
        private readonly DependencyBuilder _builder;

        public AskTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();
        }

        [Theory, AutoData]
        public async Task Ask(int id, int number)
        {
            var container = _builder.AddQueryHandler<GetBooHandler>().BuildContainer();

            var repository = container.Resolve<IBooRepository>();
            var emitter = container.Resolve<Emitter>();

            repository.AddElement(new Boo {Id = id, Int = number});

            var boo = await emitter.AskAsync(new GetBoo {Id = id});

            Assert.Equal(id, boo.Id);
            Assert.Equal(number, boo.Int);
        }

        [Theory, AutoData]
        public async Task Ask_Anonymous(int id)
        {
            var container = _builder
                .AddQueryHandler<GetBoo, Boo>((ctx, payload) => ctx
                    .Resolve<IBooRepository>()
                    .GetElement(payload.Id))
                .BuildContainer();

            var emitter = container.Resolve<Emitter>();
            var repository = container.Resolve<IBooRepository>();

            repository.AddElement(new Boo {Id = id});

            var boo = await emitter.AskAsync(new GetBoo {Id = id});

            Assert.Equal(id, boo.Id);
        }

        [Theory, AutoData]
        public async Task Ask_AnonymousAsync(int id)
        {
            var container = _builder
                .AddQueryHandler<GetBoo, Boo>((ctx, payload) => ctx
                    .Resolve<IBooRepository>()
                    .GetElementAsync(payload.Id))
                .BuildContainer();

            var emitter = container.Resolve<Emitter>();
            var repository = container.Resolve<IBooRepository>();

            repository.AddElement(new Boo {Id = id});

            var boo = await emitter.AskAsync(new GetBoo {Id = id});

            Assert.Equal(id, boo.Id);
        }
        
        [Theory, AutoData]
        public async Task Ask_Concrete(int id, int number)
        {
            var container = _builder.AddQueryHandler<GetBooHandler>().BuildContainer();

            var repository = container.Resolve<IBooRepository>();
            var emitter = container.Resolve<Emitter>();

            repository.AddElement(new Boo {Id = id, Int = number});

            var boo = await emitter.AskAsync<GetBoo, Boo>(new GetBoo {Id = id});

            Assert.Equal(id, boo.Id);
            Assert.Equal(number, boo.Int);
        }
        
        [Fact]
        public async Task MultipleHandler()
        {
            var container = _builder.AddQueryHandler<MultipleQueryHandler>().BuildContainer();
            var commandHandler = container.Resolve<MultipleQueryHandler>();
            var emitter = container.Resolve<Emitter>();

            await emitter.AskAsync(new GetBoo());
            await emitter.AskAsync(new GetBooInt());

            Assert.True(commandHandler.GetBooCalled);
            Assert.True(commandHandler.GetBooIntCalled);
        }

        [Fact]
        public async Task Throw_Not_Registered()
        {
            var emitter = new Emitter(new DependencyBuilder().BuildContainer());

            await Assert.ThrowsAsync<KeyNotFoundException>(() => emitter.AskAsync(new GetBoo()));
        }
        
        [Fact]
        public async Task Throw__NotSingleHandler()
        {
            var container = _builder
                .AddQueryHandler<GetBooHandler>()
                .AddQueryHandler<MultipleQueryHandler>()
                .BuildContainer();

            var emitter = container.Resolve<Emitter>();
            
            await Assert.ThrowsAsync<AmbiguousMatchException>(() => emitter.AskAsync(new GetBoo()));
        }
    }
}