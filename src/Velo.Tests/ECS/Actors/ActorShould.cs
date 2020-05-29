using FluentAssertions;
using Velo.ECS.Actors;
using Velo.TestsModels.ECS;
using Xunit;

namespace Velo.Tests.ECS.Actors
{
    public class ActorShould : ECSTestClass
    {
        private readonly Actor _actor;

        public ActorShould()
        {
            _actor = CreateActor();
        }

        [Fact]
        public void ContainsComponent()
        {
            _actor.AddComponent(new TestComponent1());
            _actor.ContainsComponent<TestComponent1>().Should().BeTrue();
        }

        [Fact]
        public void ContainsComponents()
        {
            _actor.AddComponent(new TestComponent1());
            _actor.AddComponent(new TestComponent2());
            _actor.ContainsComponents<TestComponent1, TestComponent2>().Should().BeTrue();
        }

        [Fact]
        public void RaiseComponentAdded()
        {
            using var actor = _actor.Monitor();

            _actor.AddComponent(new TestComponent1());

            actor.Should().Raise(nameof(Actor.ComponentAdded));
        }

        [Fact]
        public void RaiseComponentRemoved()
        {
            _actor.AddComponent(new TestComponent1());

            using var actor = _actor.Monitor();

            _actor.RemoveComponent<TestComponent1>();

            actor.Should().Raise(nameof(Actor.ComponentRemoved));
        }

        [Fact]
        public void NotContainsComponent()
        {
            _actor.ContainsComponent<TestComponent1>().Should().BeFalse();
        }

        [Fact]
        public void NotContainsComponents()
        {
            _actor.AddComponent(new TestComponent1());
            _actor.ContainsComponents<TestComponent1, TestComponent2>().Should().BeFalse();
        }

        [Fact]
        public void NotRemoveComponent()
        {
            _actor.RemoveComponent<TestComponent1>().Should().BeFalse();
        }

        [Fact]
        public void RemoveComponent()
        {
            _actor.AddComponent(new TestComponent1());
            _actor.RemoveComponent<TestComponent1>().Should().BeTrue();
        }

        [Fact]
        public void TryGetComponentTrue()
        {
            var component = new TestComponent1();

            _actor.AddComponent(component);

            _actor.TryGetComponent<TestComponent1>(out var exists).Should().BeTrue();
            exists.Should().Be(component);
        }

        [Fact]
        public void TryGetComponentFalse()
        {
            _actor.TryGetComponent<TestComponent1>(out var exists).Should().BeFalse();
            exists.Should().BeNull();
        }

        [Fact]
        public void TryGetComponentsTrue()
        {
            var component1 = new TestComponent1();
            var component2 = new TestComponent2();

            _actor.AddComponents(component1, component2);

            _actor
                .TryGetComponents<TestComponent1, TestComponent2>(out var exists1, out var exists2)
                .Should().BeTrue();

            exists1.Should().Be(component1);
            exists2.Should().Be(component2);
        }

        [Fact]
        public void TryGetComponentsFalse()
        {
            _actor.AddComponent(new TestComponent1());

            _actor
                .TryGetComponents<TestComponent1, TestComponent2>(out _, out _)
                .Should().BeFalse();
        }
    }
}