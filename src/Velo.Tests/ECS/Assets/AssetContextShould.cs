using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using Velo.ECS.Assets;
using Velo.ECS.Assets.Context;
using Velo.ECS.Assets.Filters;
using Velo.ECS.Assets.Groups;
using Velo.ECS.Components;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Assets
{
    public class AssetContextShould : ECSTestClass
    {
        private readonly Asset _asset;
        private readonly IAssetContext _assetContext;
        private readonly int _assetsCount;

        public AssetContextShould(ITestOutputHelper output) : base(output)
        {
            InjectComponentsArray(new IComponent[] {new TestComponent1(), new TestComponent2()});

            var assets = Fixture.CreateMany<TestAsset>().ToArray();
            _asset = assets[0];
            _assetContext = new AssetContext(assets);
            _assetsCount = assets.Length;
        }

        [Fact]
        public void AddFilter1()
        {
            var filter = Mock.Of<IAssetFilter<TestComponent1>>();
            _assetContext.AddFilter(filter);

            Assert.Same(filter, _assetContext.GetFilter<TestComponent1>());
        }

        [Fact]
        public void AddFilter2()
        {
            var filter = Mock.Of<IAssetFilter<TestComponent1, TestComponent2>>();
            _assetContext.AddFilter(filter);

            Assert.Same(filter, _assetContext.GetFilter<TestComponent1, TestComponent2>());
        }

        [Fact]
        public void AddGroup()
        {
            var actorGroup = Mock.Of<IAssetGroup<TestAsset>>();
            _assetContext.AddGroup(actorGroup);

            Assert.Same(actorGroup, _assetContext.GetGroup<TestAsset>());
        }

        [Fact]
        public void Contains()
        {
            _assetContext.Contains(_asset.Id).Should().BeTrue();
        }

        [Fact]
        public void Enumerable()
        {
            _assetContext.Should().Contain(a => a.Id == _asset.Id);
        }

        [Fact]
        public void EnumerableWhere()
        {
            _assetContext
                .Where((a, id) => a.Id == id, _asset.Id)
                .Should().ContainSingle(a => a.Id == _asset.Id);
        }

        [Fact]
        public void HasLength()
        {
            _assetContext.Length.Should().Be(_assetsCount);
        }

        [Fact]
        public void Get()
        {
            _assetContext.Get(_asset.Id).Should().Be(_asset);
        }

        [Fact]
        public void GetFilter1()
        {
            _assetContext
                .Invoking(context => context.GetFilter<TestComponent1>())
                .Should().NotThrow()
                .Which.Should().NotBeNull();
        }

        [Fact]
        public void GetFilter2()
        {
            _assetContext
                .Invoking(context => context.GetFilter<TestComponent1, TestComponent2>())
                .Should().NotThrow()
                .Which.Should().NotBeNull();
        }

        [Fact]
        public void GetGroup()
        {
            _assetContext
                .Invoking(context => context.GetGroup<TestAsset>())
                .Should().NotThrow()
                .Which.Should().NotBeNull();
        }

        [Fact]
        public void GetSingle()
        {
            _assetContext
                .Invoking(context => context.GetSingle<TestAsset>())
                .Should().NotThrow()
                .Which.Should().NotBeNull();
        }

        [Fact]
        public void NotContains()
        {
            _assetContext.Contains(-_asset.Id).Should().BeFalse();
        }

        [Fact]
        public void TryGetExists()
        {
            _assetContext.TryGet(_asset.Id, out var exists).Should().BeTrue();
            exists.Should().Be(_asset);
        }

        [Fact]
        public void TryGetNotExists()
        {
            _assetContext.TryGet(-_asset.Id, out var exists).Should().BeFalse();
            exists.Should().BeNull();
        }

        [Fact]
        public void ThrowIfGetNotExists()
        {
            _assetContext
                .Invoking<IAssetContext>(context => context.Get(-_asset.Id))
                .Should().Throw<KeyNotFoundException>();
        }
    }
}