using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.Settings;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;
using Emitting = Velo.TestsModels.Emitting;

namespace Velo.CQRS
{
    public class NotificationTests : TestBase
    {
        private readonly DependencyCollection _dependencies;

        public NotificationTests(ITestOutputHelper output) : base(output)
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
        public async Task Notification(int id, int number)
        {
            var provider = _dependencies
                .AddCommandProcessor<Emitting.Boos.Create.Processor>()
                .AddCommandProcessor<Emitting.Boos.Create.PostProcessor>()
                .AddNotificationProcessor<Emitting.Foos.Create.OnBooCreated>()
                .AddCommandProcessor<Emitting.Foos.Create.Processor>()
                .BuildProvider();

            var booRepository = provider.GetService<IBooRepository>();
            var fooRepository = provider.GetService<IFooRepository>();
            var mediator = provider.GetService<Emitter>();

            using (Measure())
            {
                var command = new Emitting.Boos.Create.Command {Id = id, Int = number};
                await mediator.Execute(command);
            }
            
            Assert.True(booRepository.Contains(id));
            Assert.True(fooRepository.Contains(id));
            Assert.Equal(ModelType.Boo, fooRepository.GetElement(id).Type);
        }
    }
}