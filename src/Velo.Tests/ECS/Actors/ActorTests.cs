using System;
using System.Collections.Generic;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS.Actors
{
    public class ActorTests : ECSTestBase
    {
        private readonly Actor _actor;
        private readonly int _actorId;
        private readonly IComponent[] _components;
        
        public ActorTests(ITestOutputHelper output) : base(output)
        {
            _components = BuildComponents();
            _actorId = 1;
            _actor = new Actor(_actorId, _components);
        }
        
        [Fact]
        public void AddComponent()
        {
            var component = new DefenceComponent();
            _actor.AddComponent(component);
            
            Assert.True(_actor.ContainsComponent<DefenceComponent>());
            Assert.True(_actor.TryGetComponent<DefenceComponent>(out var existsComponent));
            Assert.Same(component, _actor.GetComponent<DefenceComponent>());
            Assert.Same(component, existsComponent);
        }
        
        [Fact]
        public void AddComponent_EventRaised()
        {
            var component = new DefenceComponent();

            _actor.AddedComponent += (actor, addedComponent) =>
            {
                Assert.Same(_actor, actor);
                Assert.Same(component, addedComponent);
            };
                
            _actor.AddComponent(component);
        }
        
        [Fact]
        public void RemoveComponent()
        {
            _actor.RemoveComponent<HealthComponent>();
            
            Assert.False(_actor.ContainsComponent<HealthComponent>());
            Assert.False(_actor.TryGetComponent<HealthComponent>(out _));
        }
        
        [Fact]
        public void RemoveComponent_Many()
        {
            var actor = BuildActor(1);
            actor.AddComponent(new DefenceComponent());
            
            actor.RemoveComponent<ManaCostComponent>();
            actor.AddComponent(new ManaCostComponent());

            Assert.True(actor.ContainsComponent<ManaCostComponent>());
        }
        
        [Fact]
        public void RemoveComponent_EventRaised()
        {
            var component = _actor.GetComponent<HealthComponent>();

            _actor.RemovedComponent += (actor, removedComponent) =>
            {
                Assert.Same(_actor, actor);
                Assert.Same(component, removedComponent);
            };
                
            _actor.RemoveComponent<HealthComponent>();
        }
        
        [Fact]
        public void Throw_AddExistsComponent()
        {
            Assert.Throws<InvalidOperationException>(() => _actor.AddComponent(new HealthComponent()));
        }
        
        [Fact]
        public void Throw_RemoveNotExistsComponent()
        {
            Assert.Throws<KeyNotFoundException>(() => _actor.RemoveComponent<DefenceComponent>());
        }
    }
}