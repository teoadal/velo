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
    public class CommandsTests : TestBase
    {
        private readonly DependencyCollection _collection;
        
        public CommandsTests(ITestOutputHelper output) : base(output)
        {
            _collection = new DependencyCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();
        }

        [Theory, AutoData]
        public async Task Command(int id, int number)
        {
            var provider = _collection.AddCommandHandler<CreateBooHandler>().BuildProvider();
            
            var repository = provider.GetService<IBooRepository>();
            var mediator = provider.GetService<Emitter>();

            await mediator.Execute(new CreateBoo {Id = id, Int = number});

            var element = repository.GetElement(id);
            Assert.Equal(id, element.Id);
            Assert.Equal(number, element.Int);
        }
        
        [Theory, AutoData]
        public async Task Command_MultiThreading(Boo[] items)
        {
            var provider = _collection.AddCommandHandler<CreateBooHandler>().BuildProvider();
            
            var repository = provider.GetService<IBooRepository>();
            var mediator = provider.GetService<Emitter>();

            await RunTasks(items, item => mediator.Execute(new CreateBoo {Id = item.Id, Int = item.Int}));

            foreach (var item in items)
            {
                var element = repository.GetElement(item.Id);
                Assert.Equal(item.Id, element.Id);
                Assert.Equal(item.Int, element.Int);
            }
        }
        
        [Theory, AutoData]
        public async Task Command_MultiThreading_WithDifferentScopes(Boo[] items)
        {
            var provider = _collection.AddCommandHandler<CreateBooHandler>().BuildProvider();
            
            var repository = provider.GetService<IBooRepository>();

            await RunTasks(items, async item =>
                {
                    using (var scope = provider.CreateScope())
                    {
                        var mediator = scope.GetService<Emitter>();
                        await mediator.Execute(new CreateBoo {Id = item.Id, Int = item.Int});
                    }
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