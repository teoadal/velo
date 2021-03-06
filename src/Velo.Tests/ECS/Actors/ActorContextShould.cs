using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using Velo.ECS.Actors;
using Velo.ECS.Actors.Context;
using Velo.ECS.Actors.Filters;
using Velo.ECS.Actors.Groups;
using Velo.ECS.Components;
using Velo.TestsModels.ECS;
using Xunit;

namespace Velo.Tests.ECS.Actors
{
    public class ActorContextShould : ECSTestClass
    {
        private readonly IActorContext _actorContext;
        private readonly TestActor _actor;

        public ActorContextShould()
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
        public void Enumerable()
        {
            foreach (var actor in Fixture.CreateMany<Actor>())
            {
                _actorContext.Add(actor);
                _actorContext.Should().Contain(a => a.Id == actor.Id);
            }
        }

        [Fact]
        public void EnumerableMultiThreading()
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
        public void EnumerableWhere()
        {
            foreach (var actor in Fixture.CreateMany<Actor>())
            {
                _actorContext.Add(actor);
                _actorContext
                    .Where((a, id) => a.Id == id, actor.Id)
                    .Should().ContainSingle(a => a.Id == actor.Id);
            }
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

        [Fact]
        public void HasLength()
        {
            _actorContext.Add(_actor);
            _actorContext.Length.Should().BeGreaterOrEqualTo(1);
        }
        
        [Fact]
        public void NotContains()
        {
            _actorContext.Contains(-_actor.Id).Should().BeFalse();
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

        [Fact]
        public void TryGetNotExists()
        {
            _actorContext.TryGet(-_actor.Id, out var exists).Should().BeFalse();
            exists.Should().BeNull();
        }

        [Fact]
        public void ThrowIfGetNotExists()
        {
            _actorContext
                .Invoking<IActorContext>(context => context.Get(-_actor.Id))
                .Should().Throw<KeyNotFoundException>();
        }
    }
}