using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Moq;
using Velo.DependencyInjection;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Foos.Create;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;
using Processor = Velo.TestsModels.Emitting.Foos.Create.Processor;

namespace Velo.CQRS
{
    public class NotificationShould : TestClass
    {
        private readonly DependencyProvider _dependencyProvider;
        private readonly Mock<IFooRepository> _fooRepository;
        private readonly Emitter _emitter;

        public NotificationShould(ITestOutputHelper output) : base(output)
        {
            _fooRepository = new Mock<IFooRepository>();

            _dependencyProvider = new DependencyCollection()
                .AddInstance(_fooRepository.Object)
                .AddNotificationProcessor<OnBooCreated>()
                .AddCommandProcessor<Processor>()
                .AddEmitter()
                .BuildProvider();

            _emitter = _dependencyProvider.GetRequiredService<Emitter>();
        }

        [Theory, AutoData]
        public async Task Published(Notification notification)
        {
            notification.StopPropagation = false;

            await _emitter.Publish(notification);

            _fooRepository.Verify(repository => repository
                .AddElement(It.Is<Foo>(foo => foo.Int == notification.Id)));
        }

        [Theory, AutoData]
        public async Task PublishedMultiThreading(Notification[] notifications)
        {
            await RunTasks(notifications, notification =>
            {
                notification.StopPropagation = false;
                return _emitter.Publish(notification);
            });

            foreach (var notification in notifications)
            {
                _fooRepository.Verify(repository => repository
                    .AddElement(It.Is<Foo>(foo => foo.Int == notification.Id)));
            }
        }

        [Theory, AutoData]
        public async Task PublishedMultiThreadingWithDifferentScopes(Notification[] notifications)
        {
            await RunTasks(notifications, notification =>
            {
                notification.StopPropagation = false;

                using var scope = _dependencyProvider.CreateScope();
                var scopeEmitter = scope.GetRequiredService<Emitter>();
                return scopeEmitter.Publish(notification);
            });

            foreach (var notification in notifications)
            {
                _fooRepository.Verify(repository => repository
                    .AddElement(It.Is<Foo>(foo => foo.Int == notification.Id)));
            }
        }

        [Theory, AutoData]
        public async Task PublishedWithDifferentLifetimes(
            Notification[] notifications,
            DependencyLifetime commandProcessorLifetime,
            DependencyLifetime notificationProcessorLifetime)
        {
            var provider = new DependencyCollection()
                .AddInstance(_fooRepository.Object)
                .AddCommandProcessor<Processor>(commandProcessorLifetime)
                .AddNotificationProcessor<OnBooCreated>(notificationProcessorLifetime)
                .AddEmitter()
                .BuildProvider();

            var emitter = provider.GetRequiredService<Emitter>();

            foreach (var notification in notifications)
            {
                notification.StopPropagation = false;

                await emitter.Publish(notification);

                _fooRepository.Verify(repository => repository
                    .AddElement(It.Is<Foo>(foo => foo.Int == notification.Id)));
            }
        }

        [Theory, AutoData]
        public async Task StopPropagation(Notification notification)
        {
            notification.StopPropagation = true;

            await _emitter.Publish(notification);

            _fooRepository.Verify(repository => repository
                .AddElement(It.Is<Foo>(foo => foo.Int == notification.Id)), Times.Never);
        }
    }
}