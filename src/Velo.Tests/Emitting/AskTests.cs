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
    public class AskTests : TestBase
    {
        private readonly Emitter _emitter;
        private readonly IBooRepository _repository;
        
        public AskTests(ITestOutputHelper output) : base(output)
        {
            var container = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddQueryHandler<GetBooHandler>()
                .AddQueryHandler<GetBooIntHandler>()
                .AddEmitter()
                .BuildContainer();

            _emitter = container.Resolve<Emitter>();
            _repository = container.Resolve<IBooRepository>();
        }

        [Theory, AutoData]
        public async Task Ask(int id, int intValue)
        {
            _repository.AddElement(new Boo {Id = id, Int = intValue});

            var booInt = await _emitter.AskAsync(new GetBooInt {Id = id});

            Assert.Equal(intValue, booInt);
        }

        [Theory, AutoData]
        public async Task Ask_Async(int id)
        {
            _repository.AddElement(new Boo {Id = id});
            
            var boo = await _emitter.AskAsync(new GetBoo {Id = id});

            Assert.Equal(id, boo.Id);
        }

        [Fact]
        public void Throw_QueryHandler_Not_Registered()
        {
            var bus = new Emitter(new DependencyBuilder().BuildContainer());

            Assert.Throws<KeyNotFoundException>(() => bus.Ask(new GetBoo()));
        }
    }
}