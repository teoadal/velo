using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.Tests.NewECS.Actors;
using Velo.Tests.NewECS.Actors.Context;
using Velo.Tests.NewECS.Actors.Filters;
using Velo.Tests.NewECS.Actors.Groups;
using Velo.Tests.NewECS.Components;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.NewECS.Tests.Actors
{
    public class ActorContextShould : ECSTestClass
    {
        private readonly IActorContext _actorContext;
        private readonly TestActor _actor;

        public ActorContextShould(ITestOutputHelper output) : base(output)
        {
            _actorContext = new ActorContext();
            _actor = new TestActor(1);

            Fixture.Inject(new IComponent[]
            {
                new TestComponent1(),
                new TestComponent2()
            });
        }

        [Fact]
        public void AddActor()
        {
            _actorContext.Add(_actor);
            _actorContext.Should().Contain(_actor);
        }

        [Fact]
        public void AddFilter1()
        {
            var filter = Mock.Of<IActorFilter<TestComponent1>>();
            _actorContext.AddFilter(filter);

            Assert.Same(filter, _actorContext.GetFilter<TestComponent1>());
        }

        [Fact]
        public void AddFilter2()
        {
            var filter = Mock.Of<IActorFilter<TestComponent1, TestComponent2>>();
            _actorContext.AddFilter(filter);

            Assert.Same(filter, _actorContext.GetFilter<TestComponent1, TestComponent2>());
        }

        [Fact]
        public void AddGroup()
        {
            var actorGroup = Mock.Of<IActorGroup<TestActor>>();
            _actorContext.AddGroup(actorGroup);

            Assert.Same(actorGroup, _actorContext.GetGroup<TestActor>());
        }

        [Fact]
        public void AddRange()
        {
            var actors = Fixture.CreateMany<Actor>().ToArray();
            _actorContext.AddRange(actors);
            _actorContext.Should().Contain(actors);
        }

        [Fact]
        public void Contains()
        {
            _actorContext.Add(_actor);
            _actorContext.Contains(_actor.Id).Should().BeTrue();
        }

        [Fact]
        public void Clear()
        {
            _actorContext.Add(_actor);
            _actorContext.Clear();

            _actorContext.Should().NotContain(_actor);
        }

        [Fact]
        public void Enumerate()
        {
            foreach (var actor in Fixture.CreateMany<Actor>())
            {
                _actorContext.Add(actor);
                _actorContext.Should().Contain(a => a.Id == actor.Id);
            }
        }

        [Fact]
        public void EnumerateMultiThreading()
        {
            Parallel.For(0, 100, _ =>
            {
                foreach (var actor in Fixture.CreateMany<Actor>())
                {
                    _actorContext.Add(actor);
                    _actorContext.Should().Contain(a => a.Id == actor.Id);
                }
            });
        }

        [Fact]
        public void Get()
        {
            _actorContext.Add(_actor);
            _actorContext.Get(_actor.Id).Should().Be(_actor);
        }

        [Fact]
        public void GetFilter1()
        {
            _actorContext
                .Invoking(context => context.GetFilter<TestComponent1>()).Should().NotThrow()
                .Which.Should().NotBeNull();
        }

        [Fact]
        public void GetFilter2()
        {
            _actorContext
                .Invoking(context => context.GetFilter<TestComponent1, TestComponent2>()).Should().NotThrow()
                .Which.Should().NotBeNull();
        }

        [Fact]
        public void GetGroup()
        {
            _actorContext
                .Invoking(context => context.GetGroup<TestActor>()).Should().NotThrow()
                .Which.Should().NotBeNull();
        }

        [Fact]
        public void GetSingle()
        {
            _actorContext
                .Invoking(context => context.GetSingle<TestActor>()).Should().NotThrow()
                .Which.Should().NotBeNull();
        }

        [Theory, AutoData]
        public void NotContains(int actorId)
        {
            _actorContext.Contains(actorId).Should().BeFalse();
        }

        [Fact]
        public void Remove()
        {
            _actorContext.Add(_actor);
            _actorContext.Remove(_actor).Should().BeTrue();
        }

        [Fact]
        public void RaiseAddedEvent()
        {
            using var context = _actorContext.Monitor();

            _actorContext.Add(_actor);

            context.Should().Raise(nameof(IActorContext.Added));
        }

        [Fact]
        public void RaiseComponentAddedEvent()
        {
            using var context = _actorContext.Monitor();

            _actorContext.Add(_actor);
            _actor.AddComponent(new TestComponent1());

            context.Should().Raise(nameof(IActorContext.ComponentAdded));
        }

        [Fact]
        public void RaiseRemovedEvent()
        {
            using var context = _actorContext.Monitor();

            _actorContext.Add(_actor);
            _actorContext.Remove(_actor);

            context.Should().Raise(nameof(IActorContext.Removed));
        }

        [Fact]
        public void TryGetExists()
        {
            _actorContext.Add(_actor);
            _actorContext.TryGet(_actor.Id, out var exists).Should().BeTrue();
            exists.Should().Be(_actor);
        }

        [Theory, AutoData]
        public void TryGetNotExists(int actorId)
        {
            _actorContext.Add(_actor);
            _actorContext.TryGet(actorId, out var exists).Should().BeFalse();
            exists.Should().BeNull();
        }

        [Theory, AutoData]
        public void ThrowIfGetNotExists(int actorId)
        {
            _actorContext
                .Invoking(context => context.Get(actorId))
                .Should().Throw<KeyNotFoundException>();
        }
    }
}