using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Velo.Dependencies;
using Velo.Emitting.Commands;
using Velo.Emitting.Queries;
using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Boos.Emitting;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Emitting
{
    public sealed class EmitterTests: TestBase
    {
        private readonly DependencyBuilder _builder;
        
        public EmitterTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddEmitter();
        }

        [Theory, AutoData]
        public async Task ScanHandlers(int id, int number)
        {
            var container = _builder
                .Scan(scanner => scanner
                    .AssemblyOf<IBooRepository>()
                    .AddEmitterHandlers())
                .BuildContainer();

            var emitter = container.Resolve<Emitter>();
            var repository = container.Resolve<IBooRepository>();

            await emitter.ExecuteAsync(new CreateBoo {Id = id});

            var boo = repository.GetElement(id);
            Assert.NotNull(boo);
            
            await emitter.ExecuteAsync(new UpdateBoo {Id = id, Int = number});
            Assert.Equal(number, boo.Int);
        }
        
        [Theory, AutoData]
        public async Task Sync_CommandHandler(int id)
        {
            var container = _builder.AddCommandHandler<SyncCommandHandler>().BuildContainer();
            var emitter = container.Resolve<Emitter>();
            var repository = container.Resolve<IBooRepository>();

            await emitter.ExecuteAsync(new CreateBoo {Id = id});

            var boo = repository.GetElement(id);
            Assert.NotNull(boo);
        }
        
        [Theory, AutoData]
        public async Task Sync_QueryHandler(int id)
        {
            var container = _builder.AddQueryHandler<SyncQueryHandler>().BuildContainer();
            var emitter = container.Resolve<Emitter>();
            var repository = container.Resolve<IBooRepository>();

            repository.AddElement(new Boo {Id = id});
            
            var boo = await emitter.AskAsync(new GetBoo {Id = id});
            Assert.NotNull(boo);
        }
        
        private sealed class SyncCommandHandler: CommandHandler<CreateBoo>
        {
            private readonly IBooRepository _repository;

            public SyncCommandHandler(IBooRepository repository)
            {
                _repository = repository;
            }

            protected override void Execute(CreateBoo command)
            {
                _repository.AddElement(new Boo {Id = command.Id});
            }
        }
        
        private sealed class SyncQueryHandler: QueryHandler<GetBoo, Boo>
        {
            private readonly IBooRepository _repository;

            public SyncQueryHandler(IBooRepository repository)
            {
                _repository = repository;
            }

            protected override Boo Execute(GetBoo query)
            {
                return _repository.GetElement(query.Id);
            }
        }
    }
}