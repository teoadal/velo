using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Velo.Extensions.DependencyInjection.CQRS;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Get;
using Velo.TestsModels.Emitting.PingPong;
using Velo.TestsModels.Emitting.Plus;
using Xunit;
using Xunit.Abstractions;

namespace Velo.CQRS
{
    public class ServiceCollectionShould : TestClass
    {
        private readonly IServiceCollection _serviceCollection;

        public ServiceCollectionShould(ITestOutputHelper output) : base(output)
        {
            var repository = new Mock<IBooRepository>();

            repository
                .Setup(r => r.GetElement(It.IsAny<int>()))
                .Returns<int>(id => new Boo {Id = id});

            _serviceCollection = new ServiceCollection()
                .AddSingleton(repository.Object)
                .AddEmitter();
        }

        [Fact]
        public void ResolveEmitter()
        {
            var emitter = _serviceCollection
                .BuildServiceProvider()
                .GetService<Emitter>();

            emitter.Should().NotBeNull();
        }

        [Fact]
        public async Task ResolveNotificationPipeline()
        {
            var emitter = _serviceCollection
                .AddNotificationProcessor<PlusNotificationProcessor>()
                .BuildServiceProvider()
                .GetRequiredService<Emitter>();

            var notification = new PlusNotification();
            await emitter.Publish(notification);

            notification.Counter.Should().Be(1);
        }

        [Theory, AutoData]
        public async Task ResolveNotificationPipelineWithManyProcessors(uint count)
        {
            for (var i = 0; i < count; i++)
            {
                _serviceCollection.AddNotificationProcessor<PlusNotificationProcessor>();
            }

            var emitter = _serviceCollection
                .BuildServiceProvider()
                .GetRequiredService<Emitter>();

            for (var i = 0; i < 100; i++)
            {
                var notification = new PlusNotification();
                await emitter.Publish(notification);

                notification.Counter.Should().Be((int) count);
            }
        }

        [Fact]
        public async Task ResolveQueryPipeline()
        {
            var emitter = _serviceCollection
                .AddQueryProcessor<PingPongProcessor>()
                .BuildServiceProvider()
                .GetRequiredService<Emitter>();

            var response = await emitter.Ask(new Ping(1));
            response.Message.Should().Be(2);
        }

        [Theory, AutoData]
        public async Task ResolveFullQueryPipeline(int booId)
        {
            var emitter = _serviceCollection
                .AddQueryBehaviour<Behaviour>()
                .AddQueryProcessor<PreProcessor>()
                .AddQueryProcessor<Processor>()
                .AddQueryProcessor<PostProcessor>()
                .BuildServiceProvider()
                .GetRequiredService<Emitter>();

            var query = new Query(booId);
            var response = await emitter.Ask(query);

            query.Measured.Should().BeTrue();
            query.PreProcessed.Should().BeTrue();
            query.PostProcessed.Should().BeTrue();

            response.Id.Should().Be(booId);
        }
    }
}