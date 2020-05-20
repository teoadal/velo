using System;
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
using Velo.TestsModels.Emitting.Parallel;
using Xunit;
using Behaviour = Velo.TestsModels.Emitting.Boos.Get.Behaviour;
using Processor = Velo.TestsModels.Emitting.Boos.Create.Processor;

namespace Velo.Tests.CQRS
{
    public class EmitterInstallerShould : CQRSTestClass
    {
        private readonly DependencyCollection _dependencies;

        public EmitterInstallerShould()
        {
            _dependencies = new DependencyCollection();
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddCommandBehaviour(DependencyLifetime lifetime)
        {
            _dependencies.AddCommandBehaviour<TestsModels.Emitting.MeasureBehaviour>(lifetime);

            var behaviourType = typeof(TestsModels.Emitting.MeasureBehaviour);

            _dependencies.Contains(behaviourType).Should().BeTrue();
            _dependencies.GetLifetime(behaviourType).Should().Be(lifetime);
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddCommandProcessor(DependencyLifetime lifetime)
        {
            _dependencies.AddCommandProcessor<Processor>(lifetime);

            var processorType = typeof(Processor);

            _dependencies.Contains(processorType).Should().BeTrue();
            _dependencies.GetLifetime(processorType).Should().Be(lifetime);
        }

        [Fact]
        public void AddCommandProcessorWithManyInterfaces()
        {
            var processor = new Mock<IDisposable>()
                .As<ICommandProcessor<TestsModels.Emitting.Foos.Create.Command>>()
                .As<ICommandPreProcessor<TestsModels.Emitting.Boos.Update.Command>>()
                .As<ICommandProcessor<TestsModels.Emitting.Boos.Update.Command>>()
                .As<ICommandPostProcessor<TestsModels.Emitting.Boos.Update.Command>>()
                .As<ICommandProcessor<Command>>();

            _dependencies.AddCommandProcessor(processor.Object);

            _dependencies.Contains(typeof(ICommandProcessor<TestsModels.Emitting.Foos.Create.Command>));
            _dependencies.Contains(typeof(ICommandPreProcessor<TestsModels.Emitting.Boos.Update.Command>));
            _dependencies.Contains(typeof(ICommandProcessor<TestsModels.Emitting.Boos.Update.Command>));
            _dependencies.Contains(typeof(ICommandPostProcessor<TestsModels.Emitting.Boos.Update.Command>));
            _dependencies.Contains(typeof(ICommandProcessor<Command>));
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddNotificationProcessor(DependencyLifetime lifetime)
        {
            _dependencies.AddNotificationProcessor<NotificationProcessor>(lifetime);

            var processorType = typeof(NotificationProcessor);

            _dependencies.Contains(processorType).Should().BeTrue();
            _dependencies.GetLifetime(processorType).Should().Be(lifetime);
        }

        [Fact]
        public void AddNotificationProcessorWithManyInterfaces()
        {
            var processor = new Mock<IDisposable>()
                .As<INotificationProcessor<ParallelNotification>>()
                .As<INotificationProcessor<Notification>>();

            _dependencies.AddNotificationProcessor(processor.Object);

            _dependencies.Contains<INotificationProcessor<ParallelNotification>>();
            _dependencies.Contains<INotificationProcessor<Notification>>();
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddQueryBehaviour(DependencyLifetime lifetime)
        {
            _dependencies.AddQueryBehaviour<Behaviour>(lifetime);

            var behaviourType = typeof(Behaviour);

            _dependencies.Contains(behaviourType).Should().BeTrue();
            _dependencies.GetLifetime(behaviourType).Should().Be(lifetime);
        }

        [Theory]
        [MemberData(nameof(Lifetimes))]
        public void AddQueryProcessor(DependencyLifetime lifetime)
        {
            _dependencies.AddQueryProcessor<TestsModels.Emitting.Boos.Get.Processor>(lifetime);

            var processorType = typeof(TestsModels.Emitting.Boos.Get.Processor);

            _dependencies.Contains(processorType).Should().BeTrue();
            _dependencies.GetLifetime(processorType).Should().Be(lifetime);
        }

        [Fact]
        public void AddQueryProcessorWithManyInterfaces()
        {
            var processor = new Mock<IDisposable>()
                .As<IQueryPreProcessor<Query, Boo>>()
                .As<IQueryProcessor<Query, Boo>>()
                .As<IQueryPostProcessor<Query, Boo>>();

            _dependencies.AddQueryProcessor(processor.Object);

            _dependencies.Contains<IQueryPreProcessor<Query, Boo>>();
            _dependencies.Contains<IQueryProcessor<Query, Boo>>();
            _dependencies.Contains<IQueryPostProcessor<Query, Boo>>();
        }

        [Fact]
        public void CreateCommandProcessor()
        {
            var processor = new Mock<Action<Command>>();
            _dependencies.CreateCommandProcessor(processor.Object);
            _dependencies.Contains<ActionCommandProcessor<Command>>();
        }

        [Fact]
        public void CreateQueryProcessor()
        {
            var processor = new Mock<Func<Query, Boo>>();
            _dependencies.CreateQueryProcessor(processor.Object);
            _dependencies.Contains<ActionQueryProcessor<Query, Boo>>();
        }

        [Fact]
        public void RegisterEmitterAsScoped()
        {
            _dependencies.AddEmitter();

            _dependencies.GetLifetime<IEmitter>().Should().Be(DependencyLifetime.Scoped);
        }

        [Fact]
        public void ThrowIfAddNotCommandBehaviour()
        {
            _dependencies
                .Invoking(dependencies => dependencies.AddCommandBehaviour<Boo>())
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowIfAddNotCommandProcessor()
        {
            _dependencies
                .Invoking(dependencies => dependencies.AddCommandProcessor<Boo>())
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowIfAddNotNotificationProcessor()
        {
            _dependencies
                .Invoking(dependencies => dependencies.AddNotificationProcessor<Boo>())
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowIfAddNotQueryBehaviour()
        {
            _dependencies
                .Invoking(dependencies => dependencies.AddQueryBehaviour<Boo>())
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowIfAddNotQueryProcessor()
        {
            _dependencies
                .Invoking(dependencies => dependencies.AddQueryProcessor<Boo>())
                .Should().Throw<InvalidOperationException>();
        }
    }
}