using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Velo.Tests.NewECS.Actors;
using Velo.Tests.NewECS.Actors.Context;
using Velo.Tests.NewECS.Actors.Factory;
using Velo.Tests.NewECS.Components;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.NewECS.Tests.Actors
{
    public class ActorFactoryShould : ECSTestClass
    {
        private readonly Mock<IActorBuilder<TestActor>> _actorBuilder;
        private readonly IActorFactory _actorFactory;

        public ActorFactoryShould(ITestOutputHelper output) : base(output)
        {
            _actorBuilder = new Mock<IActorBuilder<TestActor>>();
            _actorBuilder
                .Setup(builder => builder.Build(It.IsAny<int>(), It.IsAny<IComponent[]>()))
                .Returns<int, IComponent[]>((id, components) => new TestActor(id, components));

            _actorFactory = new ActorFactory(
                Mock.Of<IActorContext>(),
                Mock.Of<IComponentFactory>(),
                new IActorBuilder[] {_actorBuilder.Object});
        }

        [Fact]
        public void ConfigureActor()
        {
            const int actorId = 25;
            var actor = _actorFactory.Configure()
                .AddComponent(Mock.Of<IComponent>())
                .AddComponent(new TestComponent1())
                .AddComponent(new TestComponent2())
                .SetId(actorId)
                .Build();
            
            actor.Id.Should().Be(actorId);
            actor.ContainsComponents<TestComponent1, TestComponent2>().Should().BeTrue();
        }

        [Fact]
        public void ConfigureTypedActor()
        {
            var actor = _actorFactory.Configure()
                .Build<TestActor>();
            
            actor.Should().BeOfType<TestActor>();
            actor.ContainsComponent<TestComponent1>().Should().BeFalse();
        }
        
        [Fact]
        public void CreateActor()
        {
            var actor = _actorFactory.Create();
            actor.Should().NotBeNull();
        }

        [Fact]
        public void CreateTypedActor()
        {
            var actor = _actorFactory.Create<TestActor>();

            _actorBuilder
                .Verify(builder => builder
                    .Build(It.IsAny<int>(), It.IsAny<IComponent[]>()));

            actor.Should().NotBeNull();
        }

        [Fact]
        public void CreateActorsWithUniqueIds()
        {
            var existsIds = new HashSet<int>();

            for (var i = 0; i < 100; i++)
            {
                var actor = _actorFactory.Create();
                existsIds.Add(actor.Id).Should().BeTrue();
            }
        }

        [Fact]
        public void CreateActorsParallel()
        {
            var existsIds = new ConcurrentDictionary<int, Actor>();

            Parallel.For(0, 100, _ =>
            {
                var actor = _actorFactory.Create();
                existsIds.TryAdd(actor.Id, actor).Should().BeTrue();
            });
        }
    }
}