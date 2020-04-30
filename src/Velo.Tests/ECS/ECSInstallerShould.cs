using System;
using AutoFixture;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.ECS.Actors.Context;
using Velo.ECS.Actors.Factory;
using Velo.ECS.Assets;
using Velo.ECS.Assets.Sources;
using Velo.ECS.Components;
using Velo.ECS.Systems;
using Velo.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS
{
    // ReSharper disable once InconsistentNaming
    public class ECSInstallerShould : ECSTestClass
    {
        private readonly DependencyCollection _dependencies;

        public ECSInstallerShould(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection()
                .AddECS();
        }

        [Fact]
        public void InstallActors()
        {
            _dependencies.Contains<IActorContext>().Should().BeTrue();
            _dependencies.Contains<IActorFactory>().Should().BeTrue();
        }

        [Fact]
        public void InstallComponentFactory()
        {
            _dependencies.Contains<IComponentFactory>().Should().BeTrue();
        }

        [Fact]
        public void InstallSystemService()
        {
            _dependencies.Contains<ISystemService>().Should().BeTrue();
        }

        [Fact]
        public void InstallAssetSource()
        {
            var dependency = _dependencies
                .AddAssets(Mock.Of<IAssetSource>())
                .GetRequiredDependency<IAssetSource>();

            dependency.Contracts.Should().Contain(typeof(IAssetSource));
            dependency.Lifetime.Should().Be(DependencyLifetime.Singleton);
        }

        [Fact]
        public void InstallAssets()
        {
            var dependency = _dependencies
                .AddAssets(_ => new[] {new Asset(1, Array.Empty<IComponent>())})
                .GetRequiredDependency<IAssetSource>();

            dependency.Contracts.Should().Contain(typeof(IAssetSource));
            dependency.Lifetime.Should().Be(DependencyLifetime.Singleton);
            dependency.Resolver.Implementation.Should().Be(typeof(AssetDelegateSource));
        }

        [Fact]
        public void InstallJsonAssets()
        {
            var dependency = _dependencies
                .AddJsonAssets(Fixture.Create<string>())
                .GetRequiredDependency<IAssetSource>();

            dependency.Contracts.Should().Contain(typeof(IAssetSource));
            dependency.Lifetime.Should().Be(DependencyLifetime.Singleton);
            dependency.Resolver.Implementation.Should().Be(typeof(AssetJsonFileSource));
        }

        [Fact]
        public void InstallJson()
        {
            _dependencies.Contains<JConverter>().Should().BeTrue();
        }
    }
}