using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Velo.ECS.Assets;
using Velo.ECS.Assets.Groups;
using Velo.ECS.Components;
using Velo.TestsModels.ECS;
using Xunit;

namespace Velo.Tests.ECS.Assets
{
    public class AssetGroupShould : ECSTestClass
    {
        private readonly TestAsset _asset;
        private readonly int _assetCount;
        private readonly IAssetGroup<TestAsset> _assetGroup;

        public AssetGroupShould()
        {
            InjectComponentsArray(Array.Empty<IComponent>());

            var assets = Fixture.CreateMany<TestAsset>().ToArray();

            _asset = assets[0];
            _assetCount = assets.Length;
            _assetGroup = new AssetGroup<TestAsset>(assets.Cast<Asset>().ToArray());
        }

        [Fact]
        public void Contains()
        {
            _assetGroup.Contains(_asset.Id).Should().BeTrue();
        }

        [Fact]
        public void Enumerable()
        {
            var exists = new HashSet<int>();

            foreach (var asset in _assetGroup)
            {
                exists.Add(asset.Id).Should().BeTrue();
            }

            exists.Count.Should().Be(_assetCount);
        }

        [Fact]
        public void EnumerableWhere()
        {
            _assetGroup
                .Where((a, id) => a.Id == id, _asset.Id)
                .Should().ContainSingle(a => a.Id == _asset.Id);
        }

        [Fact]
        public void HasLength()
        {
            _assetGroup.Length.Should().Be(_assetCount);
        }

        [Fact]
        public void NotContains()
        {
            var valid = new TestAsset(1, Array.Empty<IComponent>());
            var notValid = new Asset(2, Array.Empty<IComponent>());

            var filter = new AssetGroup<TestAsset>(new[] {valid, notValid});

            filter.Should().Contain(asset => asset.Id == valid.Id);
            filter.Should().NotContain(asset => asset.Id == notValid.Id);
        }

        [Fact]
        public void TryGetTrue()
        {
            _assetGroup.TryGet(_asset.Id, out var exists).Should().BeTrue();
            exists.Should().Be(_asset);
        }

        [Fact]
        public void TryGetFalse()
        {
            _assetGroup.TryGet(-_asset.Id, out _).Should().BeFalse();
        }
    }
}