using FluentAssertions;
using Velo.DependencyInjection;
using Velo.ECS.Actors.Context;
using Velo.ECS.Actors.Factory;
using Velo.ECS.Components;
using Velo.ECS.Systems;
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
    }
}