using System;
using System.Collections.Generic;
using System.Threading;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.CQRS;
using Velo.CQRS.Commands;
using Velo.CQRS.Notifications;
using Velo.CQRS.Queries;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS
{
    public class EmitterShould : TestClass
    {
        private readonly CancellationToken _ct;
        private readonly IEmitter _emitter;
        private Func<Type, object> _serviceResolver;

        public EmitterShould(ITestOutputHelper output) : base(output)
        {
            _ct = CancellationToken.None;

            var scope = new Mock<IServiceProvider>();
            scope
                .Setup(s => s.GetService(It.IsNotNull<Type>()))
                .Returns<Type>(type => _serviceResolver(type));

            _emitter = new Emitter(scope.Object);
        }

        [Theory, AutoData]
        public void AskedQuery(Query query)
        {
            var pipeline = new Mock<IQueryPipeline<Boo>>();

            _serviceResolver = _ => pipeline.Object;

            _emitter.Awaiting(e => e.Ask(query, _ct))
                .Should().NotThrow();

            pipeline.Verify(p => p.GetResponse(query, _ct));
        }

        [Theory, AutoData]
        public void AskedConcreteQuery(Query query)
        {
            var processor = new Mock<IQueryProcessor<Query, Boo>>();

            _serviceResolver = _ => new QueryPipeline<Query, Boo>(processor.Object);

            _emitter.Awaiting(e => e.Ask<Query, Boo>(query, _ct))
                .Should().NotThrow();

            processor.Verify(p => p.Process(query, _ct));
        }

        [Theory, AutoData]
        public void ExecuteCommand(Command command)
        {
            var processor = new Mock<ICommandProcessor<Command>>();

            _serviceResolver = _ => new CommandPipeline<Command>(processor.Object);

            _emitter.Awaiting(e => e.Execute(command, _ct))
                .Should().NotThrow();

            processor.Verify(p => p.Process(command, _ct));
        }

        [Fact]
        public void NotThrowIfNotificationProcessorNotFound()
        {
            var notification = new Notification();

            _serviceResolver = _ => null;

            _emitter.Awaiting(e => e.Publish(notification, _ct))
                .Should().NotThrow();
        }

        [Fact]
        public void PublishNotification()
        {
            var notification = new Notification();
            var processor = new Mock<INotificationProcessor<Notification>>();

            _serviceResolver = _ => new NotificationPipeline<Notification>(processor.Object);

            _emitter.Awaiting(e => e.Publish(notification, _ct))
                .Should().NotThrow();

            processor.Verify(p => p.Process(notification, _ct));
        }

        [Theory, AutoData]
        public void SendCommand(Command command)
        {
            var processor = new Mock<ICommandProcessor<Command>>();

            _serviceResolver = _ => new CommandPipeline<Command>(processor.Object);

            _emitter.Awaiting(e => e.Send(command, _ct))
                .Should().NotThrow();

            processor.Verify(p => p.Process(command, _ct));
        }

        [Fact]
        public void SendNotification()
        {
            var notification = new Notification();
            var processor = new Mock<INotificationProcessor<Notification>>();

            _serviceResolver = _ => new NotificationPipeline<Notification>(processor.Object);

            _emitter.Awaiting(e => e.Send(notification, _ct))
                .Should().NotThrow();

            processor.Verify(p => p.Process(notification, _ct));
        }

        [Theory, AutoData]
        public void ThrowIfCommandProcessorNotRegistered(Command command)
        {
            var emitter = new DependencyCollection()
                .AddEmitter()
                .BuildProvider()
                .GetRequiredService<IEmitter>();

            emitter.Awaiting(e => e.Execute(command, _ct))
                .Should().Throw<KeyNotFoundException>();

            emitter.Awaiting(e => e.Send(command, _ct))
                .Should().Throw<KeyNotFoundException>();
        }

        [Theory, AutoData]
        public void ThrowIfQueryProcessorNotRegistered(Query query)
        {
            var emitter = new DependencyCollection()
                .AddEmitter()
                .BuildProvider()
                .GetRequiredService<IEmitter>();

            emitter.Awaiting(e => e.Ask(query, _ct))
                .Should().Throw<KeyNotFoundException>();

            emitter.Awaiting(e => e.Ask<Query, Boo>(query, _ct))
                .Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            ((Emitter) _emitter).Dispose();

            _emitter
                .Awaiting(e => e.Ask(It.IsAny<Query>(), _ct))
                .Should().Throw<ObjectDisposedException>();
        }
    }
}