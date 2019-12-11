using Velo.ECS.Actors;
using Velo.ECS.Enumeration;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS
{
    public class WrapperTests : EcsTestBase
    {
        private readonly Actor _actor;
        private readonly Wrapper<Actor, HealthComponent, ManaCostComponent> _wrapper;
        private readonly Wrapper<Actor, HealthComponent> _wrapperHealth;
        
        public WrapperTests(ITestOutputHelper output) : base(output)
        {
            _actor = BuildActor(0);
            
            _wrapper = new Wrapper<Actor, HealthComponent, ManaCostComponent>(
                _actor,
                _actor.GetComponent<HealthComponent>(),
                _actor.GetComponent<ManaCostComponent>());
            
            _wrapperHealth = new Wrapper<Actor, HealthComponent>(_actor, _actor.GetComponent<HealthComponent>());
        }
        
        [Fact]
        public void Component()
        {
            var manaCost = _actor.GetComponent<ManaCostComponent>();
            var health = _actor.GetComponent<HealthComponent>();
            
            Assert.Same(health, _wrapper.Component1);
            Assert.Same(manaCost, _wrapper.Component2);
            
            Assert.Same(health, _wrapperHealth.Component1);
        }
        
        [Fact]
        public void Entity()
        {
            Assert.Same(_actor, _wrapper.Entity);
            Assert.Same(_actor, _wrapperHealth.Entity);
        }
        
        [Fact]
        public void Implicit()
        {
            Actor actor1 = _wrapper;
            Actor actor2 = _wrapperHealth;
            
            Assert.Same(_actor, actor1);
            Assert.Same(_actor, actor2);
        }
    }
}