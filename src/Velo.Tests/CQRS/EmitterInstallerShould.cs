using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Velo.CQRS;
using Velo.CQRS.Commands;
using Velo.CQRS.Queries;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Emitting.Boos.Create;
using Velo.TestsModels.Emitting.Boos.Get;
using Xunit;
using Xunit.Abstractions;
using Processor = Velo.TestsModels.Emitting.Boos.Create.Processor;

namespace Velo.Tests.CQRS
{
    public class EmitterInstallerShould : TestClass
    {
        private readonly DependencyCollection _dependencies;

        public EmitterInstallerShould(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection();
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void AddCommandBehaviour(DependencyLifetime lifetime)
        {
            _dependencies.AddCommandBehaviour<TestsModels.Emitting.MeasureBehaviour>(lifetime);

            var behaviourType = typeof(TestsModels.Emitting.MeasureBehaviour);

            _dependencies.Contains(behaviourType).Should().BeTrue();
            _dependencies.GetLifetime(behaviourType).Should().Be(lifetime);
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void AddCommandProcessor(DependencyLifetime lifetime)
        {
            _dependencies.AddCommandProcessor<Processor>(lifetime);

            var processorType = typeof(Processor);

            _dependencies.Contains(processorType).Should().BeTrue();
            _dependencies.GetLifetime(processorType).Should().Be(lifetime);
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void AddNotificationProcessor(DependencyLifetime lifetime)
        {
            _dependencies.AddNotificationProcessor<NotificationProcessor>(lifetime);

            var processorType = typeof(NotificationProcessor);

            _dependencies.Contains(processorType).Should().BeTrue();
            _dependencies.GetLifetime(processorType).Should().Be(lifetime);
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void AddQueryBehaviour(DependencyLifetime lifetime)
        {
            _dependencies.AddQueryBehaviour<Behaviour>(lifetime);

            var behaviourType = typeof(Behaviour);

            _dependencies.Contains(behaviourType).Should().BeTrue();
            _dependencies.GetLifetime(behaviourType).Should().Be(lifetime);
        }

        [Theory, MemberData(nameof(Lifetimes))]
        public void AddQueryProcessor(DependencyLifetime lifetime)
        {
            _dependencies.AddQueryProcessor<TestsModels.Emitting.Boos.Get.Processor>(lifetime);

            var processorType = typeof(TestsModels.Emitting.Boos.Get.Processor);

            _dependencies.Contains(processorType).Should().BeTrue();
            _dependencies.GetLifetime(processorType).Should().Be(lifetime);
        }

        [Fact]
        public void CreateCommandProcessor()
        {
            var processor = new Mock<Action<Command>>();
            _dependencies.CreateProcessor(processor.Object);
            _dependencies.Contains<ActionCommandProcessor<Command>>();
        }
        
        [Fact]
        public void CreateCommandProcessorWithContext()
        {
            var processor = new Mock<Action<Command, IBooRepository>>();
            _dependencies.CreateProcessor(processor.Object);
            _dependencies.Contains<ActionCommandProcessor<Command>>();
        }
        
        [Fact]
        public void CreateQueryProcessor()
        {
            var processor = new Mock<Func<Query, Boo>>();
            _dependencies.CreateProcessor(processor.Object);
            _dependencies.Contains<ActionQueryProcessor<Query, Boo>>();
        }
        
        [Fact]
        public void CreateQueryProcessorWithContext()
        {
            var processor = new Mock<Func<Query, IBooRepository, Boo>>();
            _dependencies.CreateProcessor(processor.Object);
            _dependencies.Contains<ActionQueryProcessor<Query, IBooRepository, Boo>>();
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
            Assert.Throws<InvalidOperationException>(() => _dependencies.AddCommandBehaviour<Boo>());
        }

        [Fact]
        public void ThrowIfAddNotCommandProcessor()
        {
            Assert.Throws<InvalidOperationException>(() => _dependencies.AddCommandProcessor<Boo>());
        }

        [Fact]
        public void ThrowIfAddNotNotificationProcessor()
        {
            Assert.Throws<InvalidOperationException>(() => _dependencies.AddNotificationProcessor<Boo>());
        }

        [Fact]
        public void ThrowIfAddNotQueryBehaviour()
        {
            Assert.Throws<InvalidOperationException>(() => _dependencies.AddQueryBehaviour<Boo>());
        }

        [Fact]
        public void ThrowIfAddNotQueryProcessor()
        {
            Assert.Throws<InvalidOperationException>(() => _dependencies.AddQueryProcessor<Boo>());
        }

        public static IEnumerable<object[]> Lifetimes
        {
            get
            {
                yield return new object[] { DependencyLifetime.Scoped };
                yield return new object[] { DependencyLifetime.Singleton };
                yield return new object[] { DependencyLifetime.Transient };
            }
        }
    }
}