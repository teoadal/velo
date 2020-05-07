using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.ECS.Actors.Context;
using Velo.ECS.Actors.Factory;
using Velo.ECS.Assets;
using Velo.ECS.Components;
using Velo.ECS.Sources;
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
                .AddAssets(Mock.Of<IEntitySource<Asset>>())
                .GetRequiredDependency<IEntitySource<Asset>>();

            dependency.Contracts.Should().Contain(typeof(IEntitySource<Asset>));
            dependency.Lifetime.Should().Be(DependencyLifetime.Singleton);
        }

        [Fact]
        public void InstallAssets()
        {
            var dependency = _dependencies
                .AddAssets(_ => new[] {new Asset(1, Array.Empty<IComponent>())})
                .GetRequiredDependency<IEntitySource<Asset>>();

            dependency.Contracts.Should().Contain(typeof(IEntitySource<Asset>));
            dependency.Lifetime.Should().Be(DependencyLifetime.Singleton);
            dependency.Resolver.Implementation.Should().Be(typeof(DelegateSource<Asset>));
        }

        [Fact]
        public void InstallJson()
        {
            _dependencies.Contains<JConverter>().Should().BeTrue();
        }
    }
}