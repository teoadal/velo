using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.DependencyInjection;
using Velo.Logging;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Boos.Get;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;
using Emitting = Velo.TestsModels.Emitting;

namespace Velo.CQRS
{
    public class EmitterShould : TestClass
    {
        private readonly DependencyProvider _dependencyProvider;
        private readonly Emitter _emitter;
        private readonly Mock<IBooRepository> _repository;
        private readonly Mock<ILogWriter> _logger;

        public EmitterShould(ITestOutputHelper output) : base(output)
        {
            _logger = new Mock<ILogWriter>();
            _repository = new Mock<IBooRepository>();

            _dependencyProvider = new DependencyCollection()
                .AddInstance(_repository.Object)
                .AddCommandProcessor<Emitting.Boos.Create.Processor>()
                .AddLogging()
                .AddLogWriter(_logger.Object)
                .AddNotificationProcessor<NotificationProcessor>()
                .AddQueryProcessor<Emitting.Boos.Get.Processor>()
                .AddEmitter()
                .BuildProvider();

            _emitter = _dependencyProvider.GetRequiredService<Emitter>();
        }

        [Theory, AutoData]
        public async Task ExecuteCommand(Command command)
        {
            await _emitter.Execute(command);

            _repository.Verify(repository => repository
                .AddElement(It.Is<Boo>(b => b.Id == command.Id && b.Int == command.Int)));
        }

        [Fact]
        public async Task PublishNotification()
        {
            await _emitter.Publish(new Notification());
            _logger.Verify(logger => logger
                .Write(LogLevel.Debug, nameof(NotificationProcessor)));
        }

        [Theory, AutoData]
        public async Task ReturnAskResult(Boo boo)
        {
            _repository
                .Setup(repository => repository.GetElement(It.IsAny<int>()))
                .Returns(boo);

            var query = new Query(boo.Id);
            var askResult = await _emitter.Ask(query);

            askResult.Should().Be(boo);
        }

        [Theory, AutoData]
        public async Task ReturnConcreteAskResult(Boo boo)
        {
            _repository
                .Setup(repository => repository.GetElement(It.IsAny<int>()))
                .Returns(boo);

            var query = new Query(boo.Id);
            var concreteAskResult = await _emitter.Ask<Query, Boo>(query);
            concreteAskResult.Should().Be(boo);
        }

        [Fact]
        public void Scoped()
        {
            var firstScope = _dependencyProvider.CreateScope();
            var firstEmitter = firstScope.GetRequiredService<Emitter>();

            var secondScope = _dependencyProvider.CreateScope();
            var secondEmitter = secondScope.GetRequiredService<Emitter>();

            firstEmitter.Should().NotBe(secondEmitter);
        }

        [Theory, AutoData]
        public async Task SendCommand(Command command)
        {
            await _emitter.Send(command);

            _repository.Verify(repository => repository
                .AddElement(It.Is<Boo>(b => b.Id == command.Id && b.Int == command.Int)));
        }

        [Fact]
        public async Task SendNotification()
        {
            await _emitter.Publish(new Notification());
            _logger.Verify(logger => logger
                .Write(LogLevel.Debug, nameof(NotificationProcessor)));
        }

        [Theory, AutoData]
        public async Task UseScannedProcessors(Command command, Boo boo)
        {
            var fooRepository = new Mock<IFooRepository>(); // need for notification pipeline

            var provider = new DependencyCollection()
                .AddInstance(_repository.Object)
                .AddInstance(fooRepository.Object)
                .AddEmitter()
                .AddLogging()
                .Scan(scanner => scanner
                    .AssemblyOf<Emitting.Boos.Create.Processor>()
                    .AddEmitterProcessors())
                .BuildProvider();

            var emitter = provider.GetRequiredService<Emitter>();

            await emitter.Execute(command);

            _repository.Verify(repository => repository
                .AddElement(It.Is<Boo>(b => b.Id == command.Id && b.Int == command.Int)));

            fooRepository.Verify(repository => repository
                .AddElement(It.IsAny<Foo>()));

            _repository
                .Setup(repository => repository.GetElement(It.IsAny<int>()))
                .Returns(boo);

            var result = await emitter.Ask(new Query(boo.Id));
            result.Should().Be(boo);
        }

        [Fact]
        public async Task NotThrowIfNotDisposed()
        {
            using var scope = _dependencyProvider.CreateScope();
            var emitter = scope.GetRequiredService<Emitter>();
            await emitter.Execute(new Command {Id = 1});
        }

        [Fact]
        public async Task NotThrowIfNotificationProcessorNotRegistered()
        {
            using var scope = _dependencyProvider.CreateScope();
            var emitter = scope.GetRequiredService<Emitter>();

            await emitter.Send(Mock.Of<INotification>());
            await emitter.Publish(Mock.Of<Notification>());
        }

        [Fact]
        public void ThrowIfProcessorNotRegistered()
        {
            var emitter = new DependencyCollection()
                .AddEmitter()
                .BuildProvider()
                .GetRequiredService<Emitter>();

            Assert.ThrowsAsync<KeyNotFoundException>(() => emitter.Ask(Mock.Of<Query>()));
            Assert.ThrowsAsync<KeyNotFoundException>(() => emitter.Execute(Mock.Of<Command>()));
            Assert.ThrowsAsync<KeyNotFoundException>(() => emitter.Send(Mock.Of<ICommand>()));
        }

        [Fact]
        public void ThrowDisposedAfterCloseScope()
        {
            var scope = _dependencyProvider.CreateScope();
            var emitter = scope.GetRequiredService<Emitter>();
            scope.Dispose();

            Assert.ThrowsAsync<ObjectDisposedException>(() => emitter.Ask(Mock.Of<Query>()));
            Assert.ThrowsAsync<ObjectDisposedException>(() => emitter.Execute(Mock.Of<Command>()));
            Assert.ThrowsAsync<ObjectDisposedException>(() => emitter.Send(Mock.Of<ICommand>()));
            Assert.ThrowsAsync<ObjectDisposedException>(() => emitter.Send(Mock.Of<INotification>()));
        }
    }
}