using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.ECS.Actors.Context;
using Velo.ECS.Actors.Groups;
using Velo.ECS.Injection;
using Velo.TestsModels.ECS;
using Xunit;

namespace Velo.Tests.ECS.Injection
{
    public class GroupFactoryShould : ECSTestClass
    {
        private readonly GroupFactory<IActorContext> _actorGroupFactory;
        private readonly Type _contract;

        public GroupFactoryShould()
        {
            _actorGroupFactory = new GroupFactory<IActorContext>(typeof(IActorGroup));
            _contract = typeof(IActorGroup<TestActor>);
        }

        [Fact]
        public void Applicable()
        {
            _actorGroupFactory.Applicable(_contract).Should().BeTrue();
        }

        [Fact]
        public void CreateDependency()
        {
            var dependency = _actorGroupFactory.BuildDependency(_contract, Mock.Of<IDependencyEngine>());
            dependency.Should().BeOfType<EntityContextDependency<IActorContext>>();
        }

        [Fact]
        public void CreateResolvable()
        {
            var provider = new DependencyCollection()
                .AddECS()
                .BuildProvider();

            provider.Invoking(p => p.GetRequired<IActorGroup<TestActor>>())
                .Should().NotThrow()
                .Which.Should().BeOfType<ActorGroup<TestActor>>();
        }
    }
}