using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.Settings;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using Boos = Velo.TestsModels.Emitting.Boos;

namespace Velo.CQRS
{
    public class QueryTests : TestBase
    {
        private readonly DependencyCollection _dependencies;

        public QueryTests(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection()
                .AddSingleton<IConfiguration>(ctx => new Configuration())
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();
        }

        [Theory, AutoData]
        public async Task Request(int id, int number)
        {
            var provider = _dependencies
                .AddQueryProcessor<Boos.Get.Processor>()
                .BuildProvider();

            var repository = provider.GetService<IBooRepository>();
            var mediator = provider.GetService<Emitter>();

            repository.AddElement(new Boo {Id = id, Int = number});

            Boo boo;
            using (Measure())
            {
                boo = await mediator.Ask(new Boos.Get.Query {Id = id});
            }
            
            Assert.Equal(id, boo.Id);
            Assert.Equal(number, boo.Int);
        }

        [Theory, AutoData]
        public async Task MultiThreading(int id, int number)
        {
            var provider = _dependencies
                .AddQueryProcessor<Boos.Get.Processor>()
                .BuildProvider();

            var repository = provider.GetService<IBooRepository>();
            var mediator = provider.GetService<Emitter>();

            repository.AddElement(new Boo {Id = id, Int = number});

            await RunTasks(10, async () =>
            {
                var boo = await mediator.Ask(new Boos.Get.Query {Id = id});

                Assert.Equal(id, boo.Id);
                Assert.Equal(number, boo.Int);
            });
        }

        [Theory, AutoData]
        public async Task MultiThreading_WithDifferentScopes(int id, int number)
        {
            var provider = _dependencies
                .AddQueryProcessor<Boos.Get.Processor>(DependencyLifetime.Scope)
                .BuildProvider();

            var repository = provider.GetService<IBooRepository>();

            repository.AddElement(new Boo {Id = id, Int = number});

            await RunTasks(10, async () =>
            {
                using (var scope = provider.CreateScope())
                {
                    var mediator = scope.GetService<Emitter>();

                    var boo = await mediator.Ask(new Boos.Get.Query {Id = id});

                    Assert.Equal(id, boo.Id);
                    Assert.Equal(number, boo.Int);
                }
            });
        }
    }
}