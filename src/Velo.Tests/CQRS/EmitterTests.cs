using System.Security.Principal;
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
    public class EmitterTests : TestBase
    {
        private readonly DependencyCollection _collection;

        public EmitterTests(ITestOutputHelper output) : base(output)
        {
            _collection = new DependencyCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();
        }

        [Theory, AutoData]
        public async Task Scan(int id, int number)
        {
            var provider = _collection
                .Scan(scanner => scanner
                    .AssemblyOf<GetBooHandler>()
                    .AddEmitterHandlers())
                .BuildProvider();

            var mediator = provider.GetRequiredService<Emitter>();
            var repository = provider.GetRequiredService<IBooRepository>();

            await mediator.Execute(new CreateBoo {Id = id, Int = number});
            Assert.True(repository.Contains(id));

            var boo = await mediator.Ask(new GetBoo {Id = id});
            Assert.Equal(number, boo.Int);
        }
    }
}