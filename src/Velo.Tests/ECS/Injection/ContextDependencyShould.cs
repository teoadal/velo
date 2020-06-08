using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.ECS.Actors.Context;
using Velo.ECS.Actors.Filters;
using Velo.ECS.Injection;
using Velo.TestsModels.ECS;
using Xunit;

namespace Velo.Tests.ECS.Injection
{
    public class ContextDependencyShould : ECSTestClass
    {
        private readonly Mock<IActorContext> _actorContext;
        private readonly Type _contract;
        private readonly Mock<IServiceProvider> _services;

        private readonly EntityContextDependency<IActorContext> _actorContextDependency;

        public ContextDependencyShould()
        {
            _actorContext = new Mock<IActorContext>();
            _contract = typeof(IActorFilter<TestComponent1>);
            _services = new Mock<IServiceProvider>();
            _services
                .Setup(provider => provider.GetService(typeof(IActorContext)))
                .Returns(_actorContext.Object);

            _actorContextDependency = new EntityContextDependency<IActorContext>(
                _contract,
                context => context.GetFilter<TestComponent1>());
        }

        [Fact]
        public void Applicable()
        {
            _actorContextDependency.Applicable(_contract).Should().BeTrue();
        }

        [Fact]
        public void GetInstance()
        {
            _actorContextDependency.GetInstance(_contract, _services.Object);

            _actorContext.Verify(context => context.GetFilter<TestComponent1>());
        }

        [Fact]
        public void HasContracts()
        {
            _actorContextDependency.Contracts
                .Should().ContainSingle(contract => contract == _contract)
                .And.HaveCount(1);
        }

        [Fact]
        public void HasResolverImplementation()
        {
            _actorContextDependency.Resolver.Implementation.Should().Be(_contract);
        }

        [Fact]
        public void Singleton()
        {
            _actorContextDependency.Lifetime.Should().Be(DependencyLifetime.Singleton);
        }
    }
}