using System;
using FluentAssertions;
using Velo.ECS.Assets;
using Velo.ECS.Components;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Assets
{
    public class AssetShould : ECSTestClass
    {
        private readonly Asset _actor;
        private readonly TestComponent1 _component1;
        private readonly TestComponent2 _component2;

        public AssetShould(ITestOutputHelper output) : base(output)
        {
            _component1 = new TestComponent1();
            _component2 = new TestComponent2();

            _actor = new Asset(1, new IComponent[] {_component1, _component2});
        }

        [Fact]
        public void ContainsComponent()
        {
            _actor.ContainsComponent<TestComponent1>().Should().BeTrue();
        }

        [Fact]
        public void ContainsComponents()
        {
            _actor.ContainsComponents<TestComponent1, TestComponent2>().Should().BeTrue();
        }

        [Fact]
        public void NotContainsComponent()
        {
            var asset = new Asset(1, Array.Empty<IComponent>());
            asset.ContainsComponent<TestComponent1>().Should().BeFalse();
        }

        [Fact]
        public void NotContainsComponents()
        {
            var asset = new Asset(1, new IComponent[] {_component1});
            asset.ContainsComponents<TestComponent1, TestComponent2>().Should().BeFalse();
        }

        [Fact]
        public void TryGetComponentTrue()
        {
            _actor.TryGetComponent<TestComponent1>(out var exists).Should().BeTrue();
            exists.Should().Be(_component1);
        }

        [Fact]
        public void TryGetComponentFalse()
        {
            var asset = new Asset(1, new IComponent[] {_component2});
            asset.TryGetComponent<TestComponent1>(out var exists).Should().BeFalse();
            exists.Should().BeNull();
        }

        [Fact]
        public void TryGetComponentsTrue()
        {
            _actor
                .TryGetComponents<TestComponent1, TestComponent2>(out var exists1, out var exists2)
                .Should().BeTrue();

            exists1.Should().Be(_component1);
            exists2.Should().Be(_component2);
        }

        [Fact]
        public void TryGetComponentsFalse()
        {
            var asset = new Asset(1, new IComponent[] {_component1});
            asset
                .TryGetComponents<TestComponent1, TestComponent2>(out _, out _)
                .Should().BeFalse();
        }
    }
}