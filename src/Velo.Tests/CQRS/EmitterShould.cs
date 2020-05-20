using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.CQRS;
using Velo.CQRS.Commands;
using Velo.CQRS.Commands.Pipeline;
using Velo.CQRS.Notifications;
using Velo.CQRS.Notifications.Pipeline;
using Velo.CQRS.Queries;
using Velo.CQRS.Queries.Pipeline;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;

namespace Velo.Tests.CQRS
{
    public class EmitterShould : CQRSTestClass
    {
        private readonly IEmitter _emitter;
        private Func<Type, object> _serviceResolver;

        public EmitterShould()
        {
            var scope = new Mock<IServiceProvider>();
            scope
                .Setup(s => s.GetService(It.IsNotNull<Type>()))
                .Returns<Type>(type => _serviceResolver(type));

            _emitter = new Emitter(scope.Object);
        }

        [Theory]
        [AutoData]
        public void AskedQuery(Query query)
        {
            var pipeline = new Mock<IQueryPipeline<Boo>>();

            _serviceResolver = _ => pipeline.Object;

            _emitter
                .Awaiting(e => e.Ask(query, CancellationToken))
                .Should().NotThrow();

            pipeline.Verify(p => p.GetResponse(query, CancellationToken));
        }

        [Theory]
        [AutoData]
        public void AskedConcreteQuery(Query query)
        {
            var processor = new Mock<IQueryProcessor<Query, Boo>>();

            _serviceResolver = _ => new QuerySimplePipeline<Query, Boo>(processor.Object);

            _emitter
                .Awaiting(e => e.Ask<Query, Boo>(query, CancellationToken))
                .Should().NotThrow();

            processor.Verify(p => p.Process(query, CancellationToken));
        }

        [Theory]
        [AutoData]
        public void ExecuteCommand(Command command)
        {
            var processor = new Mock<ICommandProcessor<Command>>();

            _serviceResolver = _ => new CommandSimplePipeline<Command>(processor.Object);

            _emitter
                .Awaiting(e => e.Execute(command, CancellationToken))
                .Should().NotThrow();

            processor.Verify(p => p.Process(command, CancellationToken));
        }

        [Fact]
        public void NotThrowIfNotificationProcessorNotFound()
        {
            var notification = new Notification();

            _serviceResolver = _ => null;

            _emitter
                .Awaiting(e => e.Publish(notification, CancellationToken))
                .Should().NotThrow();
            
            _emitter
                .Awaiting(e => e.Send(notification, CancellationToken))
                .Should().NotThrow();
        }

        [Fact]
        public void PublishNotification()
        {
            var notification = new Notification();
            var processor = new Mock<INotificationProcessor<Notification>>();

            _serviceResolver = _ => new NotificationSimplePipeline<Notification>(processor.Object);

            _emitter
                .Awaiting(e => e.Publish(notification, CancellationToken))
                .Should().NotThrow();

            processor.Verify(p => p.Process(notification, CancellationToken));
        }

        [Theory]
        [AutoData]
        public void SendCommand(Command command)
        {
            var processor = new Mock<ICommandProcessor<Command>>();

            _serviceResolver = _ => new CommandSimplePipeline<Command>(processor.Object);

            _emitter
                .Awaiting(e => e.Send(command, CancellationToken))
                .Should().NotThrow();

            processor.Verify(p => p.Process(command, CancellationToken));
        }

        [Fact]
        public void SendNotification()
        {
            var notification = new Notification();
            var processor = new Mock<INotificationProcessor<Notification>>();

            _serviceResolver = _ => new NotificationSimplePipeline<Notification>(processor.Object);

            _emitter
                .Awaiting(e => e.Send(notification, CancellationToken))
                .Should().NotThrow();

            processor.Verify(p => p.Process(notification, CancellationToken));
        }

        [Theory]
        [AutoData]
        public void ThrowIfCommandProcessorNotRegistered(Command command)
        {
            var emitter = new DependencyCollection()
                .AddEmitter()
                .BuildProvider()
                .GetRequired<IEmitter>();

            emitter.Awaiting(e => e.Execute(command, CancellationToken))
                .Should().Throw<KeyNotFoundException>();

            emitter.Awaiting(e => e.Send(command, CancellationToken))
                .Should().Throw<KeyNotFoundException>();
        }

        [Theory]
        [AutoData]
        public void ThrowIfQueryProcessorNotRegistered(Query query)
        {
            var emitter = new DependencyCollection()
                .AddEmitter()
                .BuildProvider()
                .GetRequired<IEmitter>();

            emitter.Awaiting(e => e.Ask(query, CancellationToken))
                .Should().Throw<KeyNotFoundException>();

            emitter.Awaiting(e => e.Ask<Query, Boo>(query, CancellationToken))
                .Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            ((Emitter) _emitter).Dispose();

            _emitter
                .Awaiting(e => e.Ask(It.IsAny<Query>(), CancellationToken))
                .Should().Throw<ObjectDisposedException>();
        }
    }
}