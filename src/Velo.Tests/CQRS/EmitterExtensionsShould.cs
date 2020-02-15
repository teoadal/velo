using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.DependencyInjection;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.CQRS
{
    public class EmitterExtensionsShould : TestClass
    {
        private readonly DependencyCollection _dependencies;

        public EmitterExtensionsShould(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection();
        }

        [Theory, AutoData]
        public void AddCommandBehaviour(DependencyLifetime lifetime)
        {
            _dependencies.AddCommandBehaviour<TestsModels.Emitting.MeasureBehaviour>(lifetime);

            var behaviourType = typeof(TestsModels.Emitting.MeasureBehaviour);

            _dependencies.Contains(behaviourType).Should().BeTrue();
            _dependencies.GetLifetime(behaviourType).Should().Be(lifetime);
        }

        [Theory, AutoData]
        public void AddCommandProcessor(DependencyLifetime lifetime)
        {
            _dependencies.AddCommandProcessor<TestsModels.Emitting.Boos.Create.Processor>(lifetime);

            var processorType = typeof(TestsModels.Emitting.Boos.Create.Processor);

            _dependencies.Contains(processorType).Should().BeTrue();
            _dependencies.GetLifetime(processorType).Should().Be(lifetime);
        }

        [Theory, AutoData]
        public void AddNotificationProcessor(DependencyLifetime lifetime)
        {
            _dependencies.AddNotificationProcessor<TestsModels.Emitting.Boos.Create.NotificationProcessor>(lifetime);

            var processorType = typeof(TestsModels.Emitting.Boos.Create.NotificationProcessor);

            _dependencies.Contains(processorType).Should().BeTrue();
            _dependencies.GetLifetime(processorType).Should().Be(lifetime);
        }

        [Theory, AutoData]
        public void AddQueryBehaviour(DependencyLifetime lifetime)
        {
            _dependencies.AddQueryBehaviour<TestsModels.Emitting.Boos.Get.Behaviour>(lifetime);

            var behaviourType = typeof(TestsModels.Emitting.Boos.Get.Behaviour);

            _dependencies.Contains(behaviourType).Should().BeTrue();
            _dependencies.GetLifetime(behaviourType).Should().Be(lifetime);
        }
        
        [Theory, AutoData]
        public void AddQueryProcessor(DependencyLifetime lifetime)
        {
            _dependencies.AddQueryProcessor<TestsModels.Emitting.Boos.Get.Processor>(lifetime);

            var processorType = typeof(TestsModels.Emitting.Boos.Get.Processor);

            _dependencies.Contains(processorType).Should().BeTrue();
            _dependencies.GetLifetime(processorType).Should().Be(lifetime);
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
    }
}