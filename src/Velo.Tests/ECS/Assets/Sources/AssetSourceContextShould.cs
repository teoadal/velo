using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Velo.ECS.Assets;
using Velo.ECS.Assets.Sources;
using Velo.ECS.Components;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Assets.Sources
{
    public class AssetSourceContextShould : ECSTestClass
    {
        private readonly Asset[] _assets;
        private readonly Mock<IAssetSource>[] _sources;

        private readonly AssetSourceContext _context;

        public AssetSourceContextShould(ITestOutputHelper output) : base(output)
        {
            _assets = Many(10, i => new Asset(i, Array.Empty<IComponent>()));
            _sources = _assets.Select(asset =>
            {
                var source = new Mock<IAssetSource>();
                source
                    .Setup(s => s.GetAssets(It.IsNotNull<IAssetSourceContext>()))
                    .Returns(new[] {asset});

                return source;
            }).ToArray();

            _context = new AssetSourceContext(_sources.Select(s => s.Object).ToArray());
        }

        [Fact]
        public void CallSources()
        {
            _context
                .Invoking(c => c.GetAssets().ToArray())
                .Should().NotThrow();

            foreach (var source in _sources)
            {
                source.Verify(s => s.GetAssets(_context));
            }
        }

        [Fact]
        public void GetAsset()
        {
            _context.GetAssets(); // initialize internal enumerators

            foreach (var asset in _assets.Reverse())
            {
                _context.Get(asset.Id).Should().Be(asset);
            }
        }

        [Fact]
        public void GetAssets()
        {
            _context
                .Invoking(c => c.GetAssets().ToArray())
                .Should().NotThrow()
                .Which.Should().Contain(_assets);
        }

        [Fact]
        public void GetRecursive()
        {
        }
    }
}