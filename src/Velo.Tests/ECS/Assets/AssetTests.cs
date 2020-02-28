using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using Velo.ECS;
using Velo.ECS.Assets;
using Velo.TestsModels.ECS;
using Velo.TestsModels.ECS.Assets;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Assets
{
    public class AssetTests : EcsTestClass
    {
        private readonly Asset _asset;
        private readonly int _assetId;
        private readonly IComponent[] _components;

        public AssetTests(ITestOutputHelper output) : base(output)
        {
            _components = BuildComponent<ParametersComponent>();
            _assetId = 1;
            _asset = new Asset(_assetId, _components);
        }

        [Fact]
        public void Create()
        {
            Assert.True(_asset.ContainsComponent<ParametersComponent>());
            Assert.Equal(_assetId, _asset.Id);
            Assert.Equal(_assetId, _asset.GetHashCode());
            Assert.Same(_components[0], _asset.GetComponent<ParametersComponent>());
        }

        [Fact]
        public void Create_Implementation()
        {
            var asset = new CreatureAsset(_assetId, _components);

            Assert.True(asset.ContainsComponent<ParametersComponent>());
            Assert.Equal(_assetId, asset.Id);
            Assert.Equal(_assetId, asset.GetHashCode());
            Assert.Same(_components[0], asset.GetComponent<ParametersComponent>());
            Assert.Same(_components[0], asset.Parameters);
        }
        
        [Theory, AutoData]
        public void Create_WithoutComponents(int id)
        {
            var asset = new Asset(id, Array.Empty<IComponent>());

            Assert.False(asset.ContainsComponent<ParametersComponent>());
            Assert.False(asset.TryGetComponent<ParametersComponent>(out _));
        }

        [Fact]
        public void TryGetComponent()
        {
            Assert.True(_asset.ContainsComponent<ParametersComponent>());
            Assert.True(_asset.TryGetComponent<ParametersComponent>(out var component));
            Assert.Same(_components[0], component);
        }

        [Fact]
        public void Throw_ComponentNotFound()
        {
            var asset = new Asset(_assetId, Array.Empty<IComponent>());
            Assert.Throws<KeyNotFoundException>(() => asset.GetComponent<ParametersComponent>());
        }
    }
}