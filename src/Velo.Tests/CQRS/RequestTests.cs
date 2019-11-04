using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Boos.Emitting;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.CQRS
{
    public class RequestTests : TestBase
    {
        private readonly DependencyCollection _collection;

        public RequestTests(ITestOutputHelper output) : base(output)
        {
            _collection = new DependencyCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();
        }

        [Theory, AutoData]
        public async Task Request(int id, int number)
        {
            var provider = _collection.AddRequestHandler<GetBooHandler>().BuildProvider();

            var repository = provider.GetService<IBooRepository>();
            var mediator = provider.GetService<Emitter>();

            repository.AddElement(new Boo {Id = id, Int = number});

            var boo = await mediator.Ask(new GetBoo {Id = id});

            Assert.Equal(id, boo.Id);
            Assert.Equal(number, boo.Int);
        }

        [Theory, AutoData]
        public async Task Request_MultiThreading(int id, int number)
        {
            var provider = _collection.AddRequestHandler<GetBooHandler>().BuildProvider();

            var repository = provider.GetService<IBooRepository>();
            var mediator = provider.GetService<Emitter>();

            repository.AddElement(new Boo {Id = id, Int = number});

            await RunTasks(10, async () =>
            {
                var boo = await mediator.Ask(new GetBoo {Id = id});

                Assert.Equal(id, boo.Id);
                Assert.Equal(number, boo.Int);
            });
        }

        [Theory, AutoData]
        public async Task Request_MultiThreading_WithDifferentScopes(int id, int number)
        {
            var provider = _collection.AddRequestHandler<GetBooHandler>(DependencyLifetime.Scope).BuildProvider();

            var repository = provider.GetService<IBooRepository>();

            repository.AddElement(new Boo {Id = id, Int = number});

            await RunTasks(10, async () =>
            {
                using (var scope = provider.CreateScope())
                {
                    var mediator = scope.GetService<Emitter>();

                    var boo = await mediator.Ask(new GetBoo {Id = id});

                    Assert.Equal(id, boo.Id);
                    Assert.Equal(number, boo.Int);
                }
            });
        }
    }
}