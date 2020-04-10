using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.Tests.NewECS.Actors;
using Velo.Tests.NewECS.Actors.Context;
using Velo.Tests.NewECS.Actors.Groups;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.NewECS.Tests.Actors
{
    public class ActorGroupShould : ECSTestClass
    {
        private readonly TestActor _actor;
        private readonly Mock<IActorContext> _actorContext;
        private readonly IActorGroup<TestActor> _actorGroup;

        public ActorGroupShould(ITestOutputHelper output) : base(output)
        {
            _actor = new TestActor(1);
            _actorContext = new Mock<IActorContext>();
            _actorGroup = new ActorGroup<TestActor>(_actorContext.Object);
        }

        [Fact]
        public void AddActor()
        {
            RaiseActorAdded(_actorContext, _actor);

            _actorGroup.Should().Contain(_actor);
        }

        [Fact]
        public void Contains()
        {
            RaiseActorAdded(_actorContext, _actor);

            _actorGroup.Contains(_actor.Id).Should().BeTrue();
        }

        [Theory, AutoData]
        public void Enumerable(int length)
        {
            for (var i = 0; i < length; i++)
            {
                RaiseActorAdded(_actorContext, new TestActor(i));
            }

            var exists = new HashSet<int>();

            foreach (var actor in _actorGroup)
            {
                exists.Add(actor.Id).Should().BeTrue();
            }

            exists.Count.Should().Be(length);
        }

        [Theory, AutoData]
        public void HasLength(int length)
        {
            for (var i = 0; i < length; i++)
            {
                RaiseActorAdded(_actorContext, _actor);
            }

            _actorGroup.Length.Should().Be(length);
        }

        [Fact]
        public void NotAddActor()
        {
            var actorId = _actor.Id + 1;

            RaiseActorAdded(_actorContext, new Actor(actorId));

            _actorGroup.Contains(actorId).Should().BeFalse();
        }

        [Fact]
        public void RaiseAdded()
        {
            using var actorGroup = _actorGroup.Monitor();

            RaiseActorAdded(_actorContext, _actor);

            actorGroup.Should().Raise(nameof(IActorGroup<TestActor>.Added));
        }

        [Fact]
        public void RaiseRemoved()
        {
            using var actorGroup = _actorGroup.Monitor();

            RaiseActorAdded(_actorContext, _actor);
            RaiseActorRemoved(_actorContext, _actor);

            actorGroup.Should().Raise(nameof(IActorGroup<TestActor>.Removed));
        }

        [Fact]
        public void RemoveActor()
        {
            RaiseActorAdded(_actorContext, _actor);
            RaiseActorRemoved(_actorContext, _actor);

            _actorGroup.Should().NotContain(_actor);
        }

        [Fact]
        public void TryGetTrue()
        {
            RaiseActorAdded(_actorContext, _actor);

            _actorGroup.TryGet(_actor.Id, out var exists).Should().BeTrue();
            exists.Should().Be(_actor);
        }

        [Fact]
        public void TryGetFalse()
        {
            _actorGroup.TryGet(_actor.Id, out var exists).Should().BeFalse();
            exists.Should().BeNull();
        }
    }
}