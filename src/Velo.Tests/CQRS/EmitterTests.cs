using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.Settings;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using Emitting = Velo.TestsModels.Emitting;

namespace Velo.CQRS
{
    public class EmitterTests : TestBase
    {
        private readonly DependencyCollection _dependencies;

        public EmitterTests(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection()
                .AddSingleton<IConfiguration>(ctx => new Configuration())
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddSingleton<IFooRepository, FooRepository>()
                .AddEmitter();
        }

        [Theory, AutoData]
        public async Task Scan(int id, int number)
        {
            var provider = _dependencies
                .Scan(scanner => scanner
                    .AssemblyOf<Emitting.Boos.Create.Processor>()
                    .AddEmitterProcessors())
                .BuildProvider();

            var mediator = provider.GetRequiredService<Emitter>();
            var repository = provider.GetRequiredService<IBooRepository>();

            await mediator.Execute(new Emitting.Boos.Create.Command {Id = id, Int = number});
            Assert.True(repository.Contains(id));

            var boo = await mediator.Ask(new Emitting.Boos.Get.Query {Id = id});
            Assert.Equal(number, boo.Int);
        }
    }
}