using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS.Actors
{
    public class ActorFilterTests : EcsTestBase
    {
        private readonly ActorContext _actorContext;
        
        public ActorFilterTests(ITestOutputHelper output) : base(output)
        {
            _actorContext = new ActorContext();
            
            for (var i = 0; i < 100; i++)
            {
                _actorContext.Add(BuildActor(i));
            }
        }

        [Fact]
        public void Added_Raised()
        {
            var filter = _actorContext.GetFilter<HealthComponent>();
            
            var actor = BuildActor(1000);

            filter.Added += addedActor => Assert.Same(actor, addedActor);
                
            _actorContext.Add(actor);
        }
        
        [Fact]
        public void Added_AfterAddComponent()
        {
            var actor = BuildActor<ManaCostComponent>(1000);
            
            _actorContext.Add(actor);
            
            var filter = _actorContext.GetFilter<HealthComponent>();
            
            Assert.False(filter.Contains(actor));
            
            actor.AddComponent(new HealthComponent());
            
            Assert.True(filter.Contains(actor));
        }
        
        [Fact]
        public void Contains()
        {
            var filter = _actorContext.GetFilter<HealthComponent>();
            var actor = _actorContext.Get(0);

            Assert.True(filter.Contains(actor));
        }
        
        [Fact]
        public void Get()
        {
            const int id = 0;
            
            var filter = _actorContext.GetFilter<HealthComponent>();
            var actor = _actorContext.Get(id);
            
            Assert.Same(actor, filter.Get(0).Entity);
        }
        
        [Fact]
        public void Iteration()
        {
            var filter = _actorContext.GetFilter<HealthComponent>();
            
            var iterated = 0;
            foreach (var actor in _actorContext)
            {
                Assert.NotNull(actor);
                iterated++;
            }
            
            Assert.Equal(filter.Length, iterated);
        }
        
        [Fact]
        public void Removed_Raised()
        {
            var filter = _actorContext.GetFilter<HealthComponent>();
            
            var actor = _actorContext.Get(0);

            filter.Removed += removedActor => Assert.Same(actor, removedActor);
                
            _actorContext.Remove(actor);
        }
        
        [Fact]
        public void Remove_AfterRemoveComponent()
        {
            var actor = BuildActor(1000);
            
            _actorContext.Add(actor);
            
            var filter = _actorContext.GetFilter<HealthComponent>();
            
            Assert.True(filter.Contains(actor));
            
            actor.RemoveComponent<HealthComponent>();
            
            Assert.False(filter.Contains(actor));
        }
        
        [Fact]
        public void Where()
        {
            var filter = _actorContext.GetFilter<HealthComponent>();
            foreach (var actor in filter.Where(a => a.Entity.Id >= 10))
            {
                Assert.True(actor.Entity.Id >= 10);
            }
        }
    }
}