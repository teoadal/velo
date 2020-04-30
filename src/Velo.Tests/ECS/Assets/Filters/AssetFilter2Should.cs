using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Velo.ECS.Assets;
using Velo.ECS.Assets.Filters;
using Velo.ECS.Components;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Assets
{
    public class AssetFilter2Should : ECSTestClass
    {
        private readonly Asset _asset;
        private readonly int _assetCount;
        private readonly IAssetFilter<TestComponent1, TestComponent2> _assetFilter;

        private readonly TestComponent1 _component1;
        private readonly TestComponent2 _component2;

        public AssetFilter2Should(ITestOutputHelper output) : base(output)
        {
            _component1 = new TestComponent1();
            _component2 = new TestComponent2();
            InjectComponentsArray(new IComponent[] {_component1, _component2});

            var assets = Fixture.CreateMany<Asset>().ToArray();

            _asset = assets[0];
            _assetCount = assets.Length;
            _assetFilter = new AssetFilter<TestComponent1, TestComponent2>(assets);
        }

        [Fact]
        public void Contains()
        {
            _assetFilter.Contains(_asset.Id).Should().BeTrue();
        }

        [Fact]
        public void Enumerable()
        {
            var exists = new HashSet<int>();

            foreach (var asset in _assetFilter)
            {
                exists.Add(asset.Id).Should().BeTrue();

                asset.Component1
                    .Should().NotBeNull().And
                    .BeOfType<TestComponent1>();
            }

            exists.Count.Should().Be(_assetCount);
        }

        [Fact]
        public void EnumerableWhere()
        {
            _assetFilter
                .Where((a, id) => a.Id == id, _asset.Id)
                .Should().ContainSingle(a => a.Id == _asset.Id);
        }

        [Fact]
        public void HasLength()
        {
            _assetFilter.Length.Should().Be(_assetCount);
        }

        [Fact]
        public void NotContains()
        {
            var valid = new Asset(1, new IComponent[] {_component1, _component2});
            var notValid = new Asset(2, new IComponent[] {_component2});

            var filter = new AssetFilter<TestComponent1, TestComponent2>(new[] {valid, notValid});

            filter.Should().Contain(asset => asset.Id == valid.Id);
            filter.Should().NotContain(asset => asset.Id == notValid.Id);
        }

        [Fact]
        public void TryGetTrue()
        {
            _assetFilter.TryGet(_asset.Id, out var exists).Should().BeTrue();
            exists.Entity.Should().Be(_asset);
        }

        [Fact]
        public void TryGetFalse()
        {
            _assetFilter.TryGet(-_asset.Id, out _).Should().BeFalse();
        }
    }
}