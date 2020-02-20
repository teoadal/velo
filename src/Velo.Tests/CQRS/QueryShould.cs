using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Velo.TestsModels.Emitting.PingPong;
using Xunit;
using Xunit.Abstractions;

namespace Velo.CQRS
{
    public class QueryShould : TestClass
    {
        private readonly DependencyProvider _dependencyProvider;
        private readonly Emitter _emitter;
        private readonly Mock<IBooRepository> _repository;

        public QueryShould(ITestOutputHelper output) : base(output)
        {
            _repository = new Mock<IBooRepository>();
            _repository.Setup(r => r
                    .GetElement(It.IsAny<int>()))
                .Returns((int id) => new Boo {Id = id});

            _dependencyProvider = new DependencyCollection()
                .AddInstance(_repository.Object)
                .AddQueryBehaviour<Behaviour>()
                .AddQueryProcessor<PreProcessor>()
                .AddQueryProcessor<Processor>()
                .AddQueryProcessor<PostProcessor>()
                .AddQueryProcessor<PingPongProcessor>()
                .AddEmitter()
                .BuildProvider();

            _emitter = _dependencyProvider.GetRequiredService<Emitter>();
        }

        [Theory, AutoData]
        public async Task AskMultiThreading(Boo[] boos)
        {
            var queries = boos.Select(b => new Query(b.Id)).ToArray();
            
            var results = await RunTasks(queries, query => _emitter.Ask(query));
            
            foreach (var query in queries)
            {
                query.PreProcessed.Should().BeTrue();
                query.PostProcessed.Should().BeTrue();

                _repository.Verify(repository => repository
                    .GetElement(It.Is<int>(id => id == query.Id)));

                results.Should().Contain(b => b.Id == query.Id);
            }
        }
        
        [Theory, AutoData]
        public async Task AskMultiThreadingWithDifferentScopes(Boo[] boos)
        {
            var queries = boos.Select(b => new Query(b.Id)).ToArray();
            
            var results = await RunTasks(queries, query =>
            {
                using var scope = _dependencyProvider.CreateScope();
                var scopeEmitter = scope.GetRequiredService<Emitter>();
                return scopeEmitter.Ask(query);
            });
            
            foreach (var query in queries)
            {
                query.PreProcessed.Should().BeTrue();
                query.PostProcessed.Should().BeTrue();

                _repository.Verify(repository => repository
                    .GetElement(It.Is<int>(id => id == query.Id)));

                results.Should().Contain(b => b.Id == query.Id);
            }
        }

        [Fact]
        public async Task AskWithoutBoxing()
        {
            var pong = await _emitter.Ask<Ping, Pong>(new Ping(1));
            pong.Message.Should().Be(2);
        }
        
        [Theory, AutoData]
        public async Task ExecutedWithDifferentLifetimes(
            Boo[] boos,
            DependencyLifetime behaviourLifetime, 
            DependencyLifetime preProcessorLifetime,
            DependencyLifetime processorLifetime,
            DependencyLifetime postProcessorLifetime)
        {
            var dependencyProvider = new DependencyCollection()
                .AddInstance(_repository.Object)
                .AddQueryBehaviour<Behaviour>(behaviourLifetime)
                .AddQueryProcessor<PreProcessor>(preProcessorLifetime)
                .AddQueryProcessor<Processor>(processorLifetime)
                .AddQueryProcessor<PostProcessor>(postProcessorLifetime)
                .AddEmitter()
                .BuildProvider();

            var emitter = dependencyProvider.GetRequiredService<Emitter>();

            for (var i = 0; i < 10; i++)
            {
                var queries = boos.Select(b => new Query(b.Id));
            
                foreach (var query in queries)
                {
                    var result = await emitter.Ask(query);
                
                    query.PreProcessed.Should().BeTrue();
                    query.PostProcessed.Should().BeTrue();

                    _repository.Verify(repository => repository
                        .GetElement(It.Is<int>(id => id == query.Id)));

                    result.Id.Should().Be(query.Id);
                }    
            }
        }
        
        [Theory, AutoData]
        public async Task MeasuredByBehaviour(Query query)
        {
            await _emitter.Ask(query);
            
            var measureBehaviour = _dependencyProvider.GetRequiredService<Behaviour>();
            measureBehaviour.Elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        }

        [Theory, AutoData]
        public async Task PreProcessed(Query query)
        {
            query.PreProcessed = false;

            await _emitter.Ask(query);

            query.PreProcessed.Should().BeTrue();
        }

        [Theory, AutoData]
        public async Task PostProcessed(Query query)
        {
            query.PostProcessed = false;

            await _emitter.Ask(query);

            query.PostProcessed.Should().BeTrue();
        }

        [Theory, AutoData]
        public async Task ReturnResult(Query query)
        {
            var boo = await _emitter.Ask(query);

            _repository.Verify(r => r
                .GetElement(It.Is<int>(id => id == query.Id)));

            boo.Id.Should().Be(query.Id);
        }
        
        [Fact]
        public async Task ReturnFromActionQueryProcessor()
        {
            var emitter = new DependencyCollection()
                .AddEmitter()
                .AddQueryProcessor<Query, Boo>(q => new Boo { Id = q.Id })
                .BuildProvider()
                .GetRequiredService<Emitter>();

            var getBooQuery = new Query {Id = 1};
            var boo = await emitter.Ask(getBooQuery);
            
            boo.Id.Should().Be(getBooQuery.Id);
        }
        
        [Fact]
        public async Task ReturnFromActionQueryProcessorWithContext()
        {
            var emitter = new DependencyCollection()
                .AddInstance(_repository.Object)
                .AddEmitter()
                .AddQueryProcessor<Query, IBooRepository, Boo>((query, repository) => repository.GetElement(query.Id))
                .BuildProvider()
                .GetRequiredService<Emitter>();

            var getBooQuery = new Query {Id = 1};
            var boo = await emitter.Ask(getBooQuery);
            
            boo.Id.Should().Be(getBooQuery.Id);
        }
        
        [Theory, AutoData]
        public async Task ThrowCancellation(Query query)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            cancellationTokenSource.Cancel();

            var emitter = new DependencyCollection()
                .AddInstance(_repository.Object)
                .AddEmitter()
                .AddQueryProcessor<Processor>()
                .BuildProvider()
                .GetRequiredService<Emitter>();

            await Assert.ThrowsAsync<OperationCanceledException>(() => emitter.Ask(query, token));
        }
    }
}