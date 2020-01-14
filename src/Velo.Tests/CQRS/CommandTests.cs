using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.Settings;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using Emitting = Velo.TestsModels.Emitting;

namespace Velo.CQRS
{
    public class CommandTests : TestBase
    {
        private readonly DependencyCollection _dependencies;

        public CommandTests(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection()
                .AddSingleton<IConfiguration>(ctx => new Configuration())
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();
        }

        [Theory, AutoData]
        public async Task Command(int id, int number)
        {
            var provider = _dependencies
                .AddCommandProcessor<Emitting.Boos.Create.Processor>()
                .BuildProvider();

            var repository = provider.GetService<IBooRepository>();
            var mediator = provider.GetService<Emitter>();

            using (Measure())
            {
                await mediator.Execute(new Emitting.Boos.Create.Command {Id = id, Int = number});
            }

            var element = repository.GetElement(id);
            Assert.Equal(id, element.Id);
            Assert.Equal(number, element.Int);
        }

        [Theory, AutoData]
        public async Task PreProcessing(int id, int number)
        {
            var provider = _dependencies
                .AddCommandProcessor<Emitting.Boos.Create.Processor>()
                .AddCommandProcessor<Emitting.Boos.Create.PreProcessor>()
                .BuildProvider();

            var mediator = provider.GetService<Emitter>();

            var command = new Emitting.Boos.Create.Command {Id = id, Int = number};

            using (Measure())
            {
                await mediator.Execute(command);
            }

            Assert.True(command.PreProcessed);
        }

        [Theory, AutoData]
        public async Task PostProcessing(int id, int number)
        {
            var provider = _dependencies
                .AddCommandProcessor<Emitting.Boos.Create.Processor>()
                .AddCommandProcessor<Emitting.Boos.Create.PostProcessor>()
                .BuildProvider();

            var mediator = provider.GetService<Emitter>();

            var command = new Emitting.Boos.Create.Command {Id = id, Int = number};

            using (Measure())
            {
                await mediator.Execute(command);
            }

            Assert.True(command.PostProcessed);
        }

        [Theory, AutoData]
        public async Task MultiThreading(Boo[] items)
        {
            var provider = _dependencies
                .AddCommandProcessor<Emitting.Boos.Create.Processor>()
                .BuildProvider();

            var repository = provider.GetService<IBooRepository>();
            var mediator = provider.GetService<Emitter>();

            await RunTasks(items, item =>
                mediator.Execute(new Emitting.Boos.Create.Command {Id = item.Id, Int = item.Int}));

            foreach (var item in items)
            {
                var element = repository.GetElement(item.Id);
                Assert.Equal(item.Id, element.Id);
                Assert.Equal(item.Int, element.Int);
            }
        }

        [Theory, AutoData]
        public async Task MultiThreading_WithDifferentScopes(Boo[] items)
        {
            var provider = _dependencies
                .AddCommandProcessor<Emitting.Boos.Create.Processor>()
                .BuildProvider();

            var repository = provider.GetService<IBooRepository>();

            await RunTasks(items, item =>
            {
                using var scope = provider.CreateScope();
                var mediator = scope.GetService<Emitter>();
                return mediator.Execute(new Emitting.Boos.Create.Command {Id = item.Id, Int = item.Int});
            });

            foreach (var item in items)
            {
                var element = repository.GetElement(item.Id);
                Assert.Equal(item.Id, element.Id);
                Assert.Equal(item.Int, element.Int);
            }
        }
    }
}