using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Velo.CQRS;
using Velo.CQRS.Commands;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting;
using Velo.TestsModels.Emitting.Parallel;
using Velo.TestsModels.Emitting.PingPong;
using Velo.TestsModels.Emitting.Plus;
using Xunit;
using Xunit.Abstractions;
using BoosCreate = Velo.TestsModels.Emitting.Boos.Create;
using BoosGet = Velo.TestsModels.Emitting.Boos.Get;

namespace Velo.Tests.Extensions.DependencyInjection.CQRS
{
    public class ServiceCollectionShould : TestClass
    {
        private readonly Mock<IBooRepository> _repository;
        private readonly IServiceCollection _serviceCollection;

        public ServiceCollectionShould(ITestOutputHelper output) : base(output)
        {
            _repository = new Mock<IBooRepository>();

            _repository
                .Setup(r => r.GetElement(It.IsAny<int>()))
                .Returns<int>(id => new Boo {Id = id});

            _serviceCollection = new ServiceCollection()
                .AddSingleton(_repository.Object)
                .AddEmitter();
        }

        [Fact]
        public void RegisterEmitterAsScoped()
        {
            var serviceDescriptor = _serviceCollection.First(s => s.ServiceType == typeof(IEmitter));
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
        }
        
        [Theory, AutoData]
        public async Task ResolveCommandPipeline(int booId)
        {
            var emitter = _serviceCollection
                .AddCommandProcessor<BoosCreate.Processor>()
                .BuildServiceProvider()
                .GetRequiredService<IEmitter>();

            await emitter.Execute(new BoosCreate.Command {Id = booId});

            _repository.Verify(repository => repository
                .AddElement(It.Is<Boo>(b => b.Id == booId)));
        }

        [Theory, AutoData]
        public async Task ResolveCommandPipelineWithBehaviour(int booId)
        {
            var emitter = _serviceCollection
                .AddSingleton(typeof(ICommandBehaviour<>), typeof(MeasureBehaviour<>))
                .AddCommandProcessor<BoosCreate.Processor>()
                .BuildServiceProvider()
                .GetRequiredService<IEmitter>();

            var command = new BoosCreate.Command {Id = booId};
            await emitter.Execute(command);

            command.Measured.Should().BeTrue();

            _repository.Verify(repository => repository
                .AddElement(It.Is<Boo>(b => b.Id == booId)));
        }

        [Theory, AutoData]
        public async Task ResolveCommandFullPipeline(int booId)
        {
            var emitter = _serviceCollection
                .AddCommandBehaviour<MeasureBehaviour<BoosCreate.Command>>()
                .AddCommandProcessor<BoosCreate.PreProcessor>()
                .AddCommandProcessor<BoosCreate.Processor>()
                .AddCommandProcessor<BoosCreate.PostProcessor>(ServiceLifetime.Scoped)
                .BuildServiceProvider()
                .GetRequiredService<IEmitter>();

            var command = new BoosCreate.Command {Id = booId};
            await emitter.Execute(command);

            command.Measured.Should().BeTrue();
            command.PreProcessed.Should().BeTrue();
            command.PostProcessed.Should().BeTrue();

            _repository.Verify(repository => repository
                .AddElement(It.Is<Boo>(b => b.Id == booId)));
        }

        [Fact]
        public void ResolveEmitter()
        {
            var emitter = _serviceCollection
                .BuildServiceProvider()
                .GetService<IEmitter>();

            emitter.Should().NotBeNull();
        }

        [Theory, AutoData]
        public async Task ResolveNotificationParallelPipeline(uint count)
        {
            for (var i = 0; i < count; i++)
            {
                _serviceCollection.AddNotificationProcessor<ParallelNotificationProcessor>();
            }

            var emitter = _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IEmitter>();

            var notification = new ParallelNotification();
            await emitter.Publish(notification);

            notification.ExecutedOn.Distinct().Should().HaveCount((int) count);
        }
        
        [Fact]
        public async Task ResolveNotificationSimplePipeline()
        {
            var emitter = _serviceCollection
                .AddNotificationProcessor<PlusNotificationProcessor>()
                .BuildServiceProvider()
                .GetRequiredService<IEmitter>();

            var notification = new PlusNotification();
            await emitter.Publish(notification);

            notification.Counter.Should().Be(1);
        }

        [Theory, AutoData]
        public async Task ResolveNotificationSequentialPipeline(uint count)
        {
            for (var i = 0; i < count; i++)
            {
                _serviceCollection.AddNotificationProcessor<PlusNotificationProcessor>();
            }

            var emitter = _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<IEmitter>();

            for (var i = 0; i < 100; i++)
            {
                var notification = new PlusNotification();
                await emitter.Publish(notification);

                notification.Counter.Should().Be((int) count);
            }
        }

        [Fact]
        public async Task ResolveQuerySimplePipeline()
        {
            var emitter = _serviceCollection
                .AddQueryProcessor<PingPongProcessor>()
                .BuildServiceProvider()
                .GetRequiredService<IEmitter>();

            var response = await emitter.Ask(new Ping(1));
            response.Message.Should().Be(2);
        }

        [Theory, AutoData]
        public async Task ResolveQuerySequentialPipeline(int booId)
        {
            var emitter = _serviceCollection
                .AddQueryProcessor<BoosGet.PreProcessor>()
                .AddQueryProcessor<BoosGet.Processor>()
                .AddQueryProcessor<BoosGet.PostProcessor>()
                .BuildServiceProvider()
                .GetRequiredService<IEmitter>();

            var query = new BoosGet.Query(booId);
            var response = await emitter.Ask(query);
            
            query.PreProcessed.Should().BeTrue();
            query.PostProcessed.Should().BeTrue();

            response.Id.Should().Be(booId);
        }
        
        [Theory, AutoData]
        public async Task ResolveQueryFullPipeline(int booId)
        {
            var emitter = _serviceCollection
                .AddQueryBehaviour<BoosGet.Behaviour>()
                .AddQueryProcessor<BoosGet.PreProcessor>()
                .AddQueryProcessor<BoosGet.Processor>()
                .AddQueryProcessor<BoosGet.PostProcessor>()
                .BuildServiceProvider()
                .GetRequiredService<IEmitter>();

            var query = new BoosGet.Query(booId);
            var response = await emitter.Ask(query);

            query.Measured.Should().BeTrue();
            query.PreProcessed.Should().BeTrue();
            query.PostProcessed.Should().BeTrue();

            response.Id.Should().Be(booId);
        }
    }
}