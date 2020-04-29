using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.ECS.Actors.Context;
using Velo.ECS.Actors.Filters;
using Velo.ECS.Injection;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Injection
{
    public class FilterFactoryShould : ECSTestClass
    {
        private readonly FilterFactory<IActorContext> _actorFilterFactory;

        public FilterFactoryShould(ITestOutputHelper output) : base(output)
        {
            _actorFilterFactory = new FilterFactory<IActorContext>(typeof(IActorFilter));
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void Applicable(Type filterType)
        {
            _actorFilterFactory.Applicable(filterType).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(FilterTypes))]
        public void CreateDependency(Type filterType)
        {
            var dependency = _actorFilterFactory.BuildDependency(filterType, Mock.Of<IDependencyEngine>());
            dependency.Should().BeOfType<ContextDependency<IActorContext>>();
        }

        [Fact]
        public void CreateResolvable()
        {
            var provider = new DependencyCollection()
                .AddECS()
                .BuildProvider();

            provider.Invoking(p => p.GetRequiredService<IActorFilter<TestComponent1>>())
                .Should().NotThrow()
                .Which.Should().BeOfType<ActorFilter<TestComponent1>>();

            provider.Invoking(p => p.GetRequiredService<IActorFilter<TestComponent1, TestComponent2>>())
                .Should().NotThrow()
                .Which.Should().BeOfType<ActorFilter<TestComponent1, TestComponent2>>();
        }

        public static TheoryData<Type> FilterTypes => new TheoryData<Type>
        {
            typeof(IActorFilter<TestComponent1>),
            typeof(IActorFilter<TestComponent1, TestComponent2>)
        };
    }
}