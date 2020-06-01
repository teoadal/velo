using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Moq;
using Velo.Collections;
using Velo.Collections.Enumerators;
using Velo.ECS.Assets;
using Velo.ECS.Components;
using Velo.ECS.Sources;
using Velo.ECS.Sources.Context;
using Velo.TestsModels.ECS;
using Xunit;

namespace Velo.Tests.ECS.Sources
{
    public class EntitySourceContextShould : ECSTestClass
    {
        private readonly IEntitySourceContext<Asset> _context;
        private readonly Mock<IEntitySource<Asset>> _source;
        private readonly IEntitySource<Asset>[] _sources;

        public EntitySourceContextShould()
        {
            _source = new Mock<IEntitySource<Asset>>();
            _source
                .Setup(source => source.GetEntities(_context))
                .Returns(Array.Empty<Asset>());

            _sources = new[] {_source.Object};
            _context = new EntitySourceContext<Asset>();
        }

        [Fact]
        public void DisposeSource()
        {
            _context
                .Invoking(context => context.GetEntities(_sources).ToArray())
                .Should().NotThrow();

            _source.Verify(source => source.Dispose());
        }

        [Fact]
        public void GetEmptyEnumeratorIfEventSourcesIsNotRegistered()
        {
            var empty = Array.Empty<IEntitySource<Asset>>();
            _context
                .GetEntities(empty)
                .Should().BeOfType<EmptyEnumerator<Asset>>();
        }

        [Fact]
        public void GetAssetFromOtherSource()
        {
            var assets = Many(100, i => CreateAsset(i));
            var sources = Many(10, i => CreateSource(assets.Skip(i * 10).Take(10)));

            const int testAssetId = 101;
            TestAsset testAsset = null;
            var delegateSource = new DelegateSource<Asset>(context =>
            {
                testAsset = new TestAsset(testAssetId, Array.Empty<IComponent>()) {Reference = context.Get(90)};
                return new[] {testAsset};
            });
            CollectionUtils.Add(ref sources, delegateSource);

            StartEnumeration(sources);

            var actual = (TestAsset) _context
                .Invoking(context => context.Get(testAssetId))
                .Should().NotThrow()
                .Which;

            actual.Should().Be(testAsset);
        }

        [Fact]
        public void GetExistsAsset()
        {
            var asset = CreateAsset(1);
            _source
                .Setup(source => source.GetEntities(_context))
                .Returns(new[] {asset});

            StartEnumeration();

            var actual = _context
                .Invoking(context => context.Get(asset.Id))
                .Should().NotThrow()
                .Which;

            actual.Should().Be(asset);
        }

        [Fact]
        public void NotStartedAfterEndEnumeration()
        {
            _context
                .Invoking(context => context.GetEntities(_sources).ToArray())
                .Should().NotThrow();

            _context.IsStarted.Should().BeFalse();
        }

        [Fact]
        public void ReturnAllEntitiesFromSource()
        {
            var assets = new[] {CreateAsset(1), CreateAsset(2), CreateAsset(3)};

            _source
                .Setup(source => source.GetEntities(_context))
                .Returns(assets);

            var actual = _context
                .Invoking(context => context.GetEntities(_sources))
                .Should().NotThrow()
                .Which.ToArray();

            // ReSharper disable once CoVariantArrayConversion
            actual.Should().BeEquivalentTo(assets);
        }

        [Fact]
        public void ReturnAllEntitiesFromSources()
        {
            var assets = Many(100, i => CreateAsset(i));
            var sources = Many(10, i => CreateSource(assets.Skip(i * 10).Take(10)));

            var actual = _context
                .Invoking(context => context.GetEntities(sources))
                .Should().NotThrow()
                .Which.ToArray();

            // ReSharper disable once CoVariantArrayConversion
            actual.Should().BeEquivalentTo(assets);
        }

        [Fact]
        public void SendContextAfterStartEnumeration()
        {
            _context
                .Invoking(context => context.GetEntities(_sources).ToArray())
                .Should().NotThrow();

            _source.Verify(source => source.GetEntities(_context));
        }

        [Fact]
        public void StartedAfterStartEnumeration()
        {
            StartEnumeration();

            _context.IsStarted.Should().BeTrue();
        }

        [Fact]
        public void ThrowGetEntitiesAgainAfterStartEnumeration()
        {
            StartEnumeration();
            
            _context
                .Invoking(context => context.GetEntities(_sources))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowGetByIdIfEnumerationNotStarted()
        {
            _context
                .Invoking(context => context.Get(It.IsAny<int>()))
                .Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ThrowIfEntityNotExists()
        {
            StartEnumeration();

            _context
                .Invoking(context => context.Get(Fixture.Create<int>()))
                .Should().Throw<KeyNotFoundException>();
        }

        private IEntitySource<Asset> CreateSource(IEnumerable<Asset> assets)
        {
            var source = new Mock<IEntitySource<Asset>>();

            source
                .Setup(s => s.GetEntities(_context))
                .Returns(assets.ToArray());

            return source.Object;
        }

        private void StartEnumeration(IEntitySource<Asset>[] sources = null)
        {
            _context.GetEntities(sources ?? _sources);
        }
    }
}