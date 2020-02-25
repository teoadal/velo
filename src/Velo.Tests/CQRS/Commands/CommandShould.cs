using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Logging;
using Velo.Logging.Writers;
using Velo.Serialization.Models;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting;
using Velo.TestsModels.Emitting.Boos.Create;
using Xunit;
using Xunit.Abstractions;

namespace Velo.CQRS.Commands
{
    public class CommandShould : TestClass
    {
        private readonly DependencyProvider _dependencyProvider;
        private readonly Emitter _emitter;
        private readonly Mock<IBooRepository> _repository;
        private readonly Mock<ILogWriter> _logger;

        public CommandShould(ITestOutputHelper output) : base(output)
        {
            _logger = new Mock<ILogWriter>();
            _repository = new Mock<IBooRepository>();

            _dependencyProvider = new DependencyCollection()
                .AddInstance(_repository.Object)
                .AddLogging()
                .AddLogWriter(_logger.Object)
                .AddCommandBehaviour<MeasureBehaviour>()
                .AddCommandBehaviour<ExceptionBehaviour<Command>>()
                .AddCommandProcessor<PreProcessor>()
                .AddCommandProcessor<Processor>()
                .AddCommandProcessor<PostProcessor>(DependencyLifetime.Scoped)
                .AddNotificationProcessor<NotificationProcessor>()
                .AddEmitter()
                .BuildProvider();

            _emitter = _dependencyProvider.GetRequiredService<Emitter>();
        }

        [Fact]
        public async Task CatchExceptionByBehaviour()
        {
            _repository
                .Setup(repository => repository.AddElement(It.Is<Boo>(boo => boo.Id == 0)))
                .Throws<InvalidOperationException>();

            await _emitter.Execute(new Command {Id = 0});

            var exceptionBehaviour = _dependencyProvider.GetRequiredService<ExceptionBehaviour<Command>>();
            exceptionBehaviour.Exception.Should().BeOfType<InvalidOperationException>();
        }

        [Fact]
        public async Task ExecuteActionCommandProcessor()
        {
            var emitter = new DependencyCollection()
                .AddEmitter()
                .AddCommandProcessor<Command>(cmd => { cmd.Id++; })
                .BuildProvider()
                .GetRequiredService<Emitter>();

            var command = new Command();
            await emitter.Execute(command);

            command.Id.Should().Be(1);
        }

        [Fact]
        public async Task ExecuteActionCommandProcessorWithContext()
        {
            var emitter = new DependencyCollection()
                .AddInstance(_repository.Object)
                .AddEmitter()
                .AddCommandProcessor<Command, IBooRepository>((cmd, repository) => repository
                    .AddElement(new Boo {Id = cmd.Id}))
                .BuildProvider()
                .GetRequiredService<Emitter>();

            var command = new Command();
            await emitter.Execute(command);

            _repository.Verify(repository => repository
                .AddElement(It.Is<Boo>(boo => boo.Id == command.Id)));
        }

        [Theory, AutoData]
        public async Task ExecutedMultiThreading(Boo[] boos)
        {
            var commands = boos
                .Select(b => new Command {Id = b.Id, Int = b.Int})
                .ToArray();

            await RunTasks(commands, command => _emitter.Execute(command));

            foreach (var command in commands)
            {
                command.Measured.Should().BeTrue();
                command.PreProcessed.Should().BeTrue();
                command.PostProcessed.Should().BeTrue();

                _repository.Verify(repository => repository
                    .AddElement(It.Is<Boo>(boo => boo.Id == command.Id && boo.Int == command.Int)));
            }
        }

        [Theory, AutoData]
        public async Task ExecutedMultiThreadingWithDifferentScopes(Boo[] boos)
        {
            var commands = boos
                .Select(b => new Command {Id = b.Id, Int = b.Int})
                .ToArray();

            await RunTasks(commands, command =>
            {
                using var scope = _dependencyProvider.CreateScope();
                var scopeEmitter = scope.GetRequiredService<Emitter>();
                return scopeEmitter.Execute(command);
            });

            foreach (var command in commands)
            {
                command.Measured.Should().BeTrue();
                command.PreProcessed.Should().BeTrue();
                command.PostProcessed.Should().BeTrue();

                _repository.Verify(repository => repository
                    .AddElement(It.Is<Boo>(boo => boo.Id == command.Id && boo.Int == command.Int)));
            }
        }

        [Theory, AutoData]
        public async Task ExecutedWithDifferentLifetimes(
            Boo[] boos,
            DependencyLifetime measureLifetime,
            DependencyLifetime exceptionLifetime,
            DependencyLifetime preProcessorLifetime,
            DependencyLifetime processorLifetime,
            DependencyLifetime postProcessorLifetime)
        {
            var dependencyCollection = new DependencyCollection();
            var dependencyProvider = dependencyCollection
                .AddInstance(_repository.Object)
                .AddCommandBehaviour<MeasureBehaviour>(measureLifetime)
                .AddCommandBehaviour<ExceptionBehaviour<Command>>(exceptionLifetime)
                .AddCommandProcessor<PreProcessor>(preProcessorLifetime)
                .AddCommandProcessor<Processor>(processorLifetime)
                .AddCommandProcessor<PostProcessor>(postProcessorLifetime)
                .AddEmitter()
                .BuildProvider();

            var expectedLifetime = new[]
                    {measureLifetime, exceptionLifetime, preProcessorLifetime, processorLifetime, postProcessorLifetime}
                .DefineLifetime();
            dependencyCollection.GetLifetime<CommandPipeline<Command>>().Should().Be(expectedLifetime);

            var emitter = dependencyProvider.GetRequiredService<Emitter>();

            for (var i = 0; i < 10; i++)
            {
                var commands = boos.Select(b => new Command {Id = b.Id, Int = b.Int});

                foreach (var command in commands)
                {
                    await emitter.Execute(command);

                    command.Measured.Should().BeTrue();
                    command.PreProcessed.Should().BeTrue();
                    command.PostProcessed.Should().BeTrue();

                    _repository.Verify(repository => repository
                        .AddElement(It.Is<Boo>(boo => boo.Id == command.Id && boo.Int == command.Int)));
                }
            }
        }

        [Fact]
        public async Task MeasuredByBehaviour()
        {
            await _emitter.Execute(Mock.Of<Command>());

            var measureBehaviour = _dependencyProvider.GetRequiredService<MeasureBehaviour>();
            measureBehaviour.Elapsed.Should().BeGreaterThan(TimeSpan.Zero);
        }

        [Fact]
        public async Task PreProcessed()
        {
            var command = new Command();

            await _emitter.Execute(command);

            command.PreProcessed.Should().BeTrue();
        }

        [Fact]
        public async Task PostProcessed()
        {
            var command = new Command();

            await _emitter.Execute(command);

            command.PostProcessed.Should().BeTrue();
        }

        [Theory, AutoData]
        public async Task RiseNotification(Command command)
        {
            await _emitter.Execute(command); // post processor must publish notification

            _logger.Verify(logger => logger
                .Write(It.Is<LogContext>(
                    context => context.Level == LogLevel.Debug && context.Sender == typeof(NotificationProcessor)), 
                    It.IsAny<JsonObject>()));
        }

        [Theory, AutoData]
        public async Task ThrowCancellation(Command command)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            cancellationTokenSource.Cancel();

            var emitter = new DependencyCollection()
                .AddInstance(_repository.Object)
                .AddEmitter()
                .AddCommandProcessor<Processor>()
                .BuildProvider()
                .GetRequiredService<Emitter>();

            await Assert.ThrowsAsync<OperationCanceledException>(() => emitter.Execute(command, token));
        }
    }
}