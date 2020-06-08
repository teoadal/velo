using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.ECS.Actors;
using Velo.ECS.Actors.Context;
using Velo.ECS.Injection;
using Velo.TestsModels.ECS;
using Xunit;

namespace Velo.Tests.ECS.Injection
{
    public class SingleFactoryShould : ECSTestClass
    {
        private readonly SingleFactory<IActorContext> _actorSingleFactory;
        private readonly Type _contract;

        public SingleFactoryShould()
        {
            _actorSingleFactory = new SingleFactory<IActorContext>(typeof(SingleActor<>));
            _contract = typeof(SingleActor<TestActor>);
        }

        [Fact]
        public void Applicable()
        {
            _actorSingleFactory.Applicable(_contract).Should().BeTrue();
        }

        [Fact]
        public void CreateDependency()
        {
            var dependency = _actorSingleFactory.BuildDependency(_contract, Mock.Of<IDependencyEngine>());
            dependency.Should().BeOfType<EntityContextDependency<IActorContext>>();
        }

        [Fact]
        public void CreateResolvable()
        {
            var provider = new DependencyCollection()
                .AddECS()
                .BuildProvider();

            provider.Invoking(p => p.GetRequired<SingleActor<TestActor>>())
                .Should().NotThrow()
                .Which.Should().BeOfType<SingleActor<TestActor>>();
        }
    }
}