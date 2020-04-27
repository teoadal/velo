using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Velo.Collections;
using Velo.ECS.Actors;
using Velo.ECS.Actors.Context;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.ECS.Actors
{
    public class SingleActorShould : ECSTestClass
    {
        private readonly TestActor _actor;
        private readonly Mock<IActorContext> _actorContext;
        private readonly SingleActor<TestActor> _single;

        public SingleActorShould(ITestOutputHelper output) : base(output)
        {
            _actor = new TestActor(1);
            _actorContext = new Mock<IActorContext>();

            _actorContext
                .Setup(context => context.GetEnumerator())
                .Returns(EmptyEnumerator<Actor>.Instance);

            _single = new SingleActor<TestActor>(_actorContext.Object);
        }

        [Fact]
        public void AddInstance()
        {
            RaiseActorAdded(_actorContext, _actor);

            _single.GetInstance().Should().Be(_actor);
        }

        [Fact]
        public void Exists()
        {
            RaiseActorAdded(_actorContext, _actor);

            _single.Exists.Should().BeTrue();
        }

        [Fact]
        public void ImplicitConversion()
        {
            RaiseActorAdded(_actorContext, _actor);

            TestActor exists = _single;
            exists.Should().Be(_actor);
        }

        [Fact]
        public void NotAddInstance()
        {
            RaiseActorAdded(_actorContext, new Actor(1));

            _single.Exists.Should().BeFalse();
        }

        [Fact]
        public void NotExists()
        {
            _single.Exists.Should().BeFalse();
        }

        [Fact]
        public void NotRemoveInstance()
        {
            RaiseActorAdded(_actorContext, _actor);
            RaiseActorRemoved(_actorContext, new Actor(1));

            _single.Exists.Should().BeTrue();
        }

        [Fact]
        public void RemoveInstance()
        {
            RaiseActorAdded(_actorContext, _actor);
            RaiseActorRemoved(_actorContext, _actor);

            _single.Exists.Should().BeFalse();
        }

        [Fact]
        public void TryGetTrue()
        {
            RaiseActorAdded(_actorContext, _actor);

            _single.TryGetInstance(out var exists).Should().BeTrue();
            exists.Should().Be(_actor);
        }

        [Fact]
        public void TryGetFalse()
        {
            _single.TryGetInstance(out var exists).Should().BeFalse();
            exists.Should().BeNull();
        }

        [Fact]
        public void ThrowNotExists()
        {
            _single
                .Invoking(single => single.GetInstance())
                .Should().Throw<KeyNotFoundException>();
        }
    }
}