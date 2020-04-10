using System.Collections.Generic;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.Tests.NewECS.Actors;
using Velo.Tests.NewECS.Actors.Context;
using Velo.Tests.NewECS.Actors.Filters;
using Velo.Tests.NewECS.Components;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.NewECS.Tests.Actors
{
    public class ActorFilter2Should : ECSTestClass
    {
        private readonly TestActor _actor;
        private readonly Mock<IActorContext> _actorContext;
        private readonly IActorFilter<TestComponent1, TestComponent2> _actorFilter;

        public ActorFilter2Should(ITestOutputHelper output) : base(output)
        {
            _actor = new TestActor(1, new IComponent[] {new TestComponent1(), new TestComponent2()});
            _actorContext = new Mock<IActorContext>();
            _actorFilter = new ActorFilter<TestComponent1, TestComponent2>(_actorContext.Object);
        }

        [Fact]
        public void AddActor()
        {
            RaiseActorAdded(_actorContext, _actor);

            _actorFilter.Contains(_actor.Id).Should().BeTrue();
        }

        [Theory, AutoData]
        public void Enumerable(int length)
        {
            for (var i = 0; i < length; i++)
            {
                var components = new IComponent[] {new TestComponent1(), new TestComponent2()};
                RaiseActorAdded(_actorContext, new TestActor(i, components));
            }

            var exists = new HashSet<int>();

            foreach (var actor in _actorFilter)
            {
                exists.Add(actor.Id).Should().BeTrue();

                actor.Component1
                    .Should().NotBeNull().And
                    .BeOfType<TestComponent1>();

                actor.Component2
                    .Should().NotBeNull().And
                    .BeOfType<TestComponent2>();
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

            _actorFilter.Length.Should().Be(length);
        }

        [Fact]
        public void NotAddActor()
        {
            var actorId = _actor.Id + 1;
            RaiseActorAdded(_actorContext, new Actor(actorId));

            _actorFilter.Contains(actorId).Should().BeFalse();
        }

        [Fact]
        public void NotRemoveActor()
        {
            RaiseActorAdded(_actorContext, _actor);

            var actorId = _actor.Id + 1;
            RaiseActorAdded(_actorContext, new Actor(actorId));

            _actorFilter.Contains(_actor.Id).Should().BeTrue();
        }

        [Fact]
        public void RaiseAdded()
        {
            using var actorFilter = _actorFilter.Monitor();

            RaiseActorAdded(_actorContext, _actor);

            actorFilter.Should().Raise(nameof(IActorFilter<TestComponent1>.Added));
        }

        [Fact]
        public void RaiseRemoved()
        {
            using var actorFilter = _actorFilter.Monitor();

            RaiseActorAdded(_actorContext, _actor);
            RaiseActorRemoved(_actorContext, _actor);

            actorFilter.Should().Raise(nameof(IActorFilter<TestComponent2>.Removed));
        }

        [Fact]
        public void RemoveActor()
        {
            RaiseActorAdded(_actorContext, _actor);
            RaiseActorRemoved(_actorContext, _actor);

            _actorFilter.Contains(_actor.Id).Should().BeFalse();
        }

        [Fact]
        public void TryGetTrue()
        {
            RaiseActorAdded(_actorContext, _actor);

            _actorFilter.TryGet(_actor.Id, out var exists).Should().BeTrue();
            exists.Entity.Should().Be(_actor);
        }

        [Fact]
        public void TryGetFalse()
        {
            _actorFilter.TryGet(_actor.Id, out _).Should().BeFalse();
        }
    }
}