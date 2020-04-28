using System;
using System.Collections.Generic;
using FluentAssertions;
using Velo.ECS.Assets;
using Velo.ECS.Components;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS.Tests.Assets
{
    public class SingleAssetShould : ECSTestClass
    {
        private readonly TestAsset _asset;
        private readonly SingleAsset<TestAsset> _single;
        
        public SingleAssetShould(ITestOutputHelper output) : base(output)
        {
            _asset = new TestAsset(1, Array.Empty<IComponent>());
            _single = new SingleAsset<TestAsset>(new Asset[] {_asset});
        }

        [Fact]
        public void GetInstance()
        {
            _single.GetInstance().Should().Be(_asset);
        }

        [Fact]
        public void ImplicitConversion()
        {
            TestAsset converted = _single;
            converted.Should().Be(_asset);
        }

        [Fact]
        public void ThrowIfInstanceNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() => new SingleAsset<TestAsset>(Array.Empty<Asset>()));
        }
    }
}