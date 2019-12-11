using Velo.TestsModels.ECS;
using Velo.TestsModels.ECS.Actors;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS.Actors
{
    public class ActorContextTests : EcsTestBase
    {
        private readonly ActorContext _actorContext;
        
        public ActorContextTests(ITestOutputHelper output) : base(output)
        {
            _actorContext = new ActorContext();
        }

        [Fact]
        public void Add()
        {
            var actor = BuildActor(0);
            
            Measure(() => _actorContext.Add(actor));

            Assert.True(_actorContext.Contains(actor));
        }

        [Fact]
        public void Add_EventRaised()
        {
            var actor = BuildActor(0);
            
            _actorContext.Added += added => Assert.Equal(actor, added);
            
            Measure(() => _actorContext.Add(actor));
        }

        [Fact]
        public void Add_FilterContains_CreatedBefore()
        {
            var actor = BuildActor(0);
            
            var filterManaCost = _actorContext.GetFilter<ManaCostComponent>();
            var filterHealth = _actorContext.GetFilter<HealthComponent>();
            var filter1 = _actorContext.GetFilter<HealthComponent, ManaCostComponent>();
            var filter2 = _actorContext.GetFilter<ManaCostComponent, HealthComponent>();

            Measure(() => _actorContext.Add(actor));

            Assert.True(filterManaCost.Contains(actor));
            Assert.True(filterHealth.Contains(actor));
            Assert.True(filter1.Contains(actor));
            Assert.True(filter2.Contains(actor));
        }

        [Fact]
        public void Add_FilterContains_CreatedAfter()
        {
            var actor = BuildActor(0);
            
            Measure(() => _actorContext.Add(actor));

            var filterManaCost = _actorContext.GetFilter<ManaCostComponent>();
            var filterHealth = _actorContext.GetFilter<HealthComponent>();
            var filter1 = _actorContext.GetFilter<HealthComponent, ManaCostComponent>();
            var filter2 = _actorContext.GetFilter<ManaCostComponent, HealthComponent>();

            Assert.True(filterManaCost.Contains(actor));
            Assert.True(filterHealth.Contains(actor));
            Assert.True(filter1.Contains(actor));
            Assert.True(filter2.Contains(actor));
        }

        [Fact]
        public void Add_GroupContains_CreatedBefore()
        {
            var actor = new Creature(0, BuildComponents());
            
            var creatures = _actorContext.GetGroup<Creature>();

            Measure(() => _actorContext.Add(actor));

            Assert.True(creatures.Contains(actor));
        }

        [Fact]
        public void Add_GroupContains_CreatedAfter()
        {
            var actor = new Creature(0, BuildComponents());
            
            Measure(() => _actorContext.Add(actor));

            var creatures = _actorContext.GetGroup<Creature>();
            Assert.True(creatures.Contains(actor));
        }

        [Fact]
        public void Iteration()
        {
            var iterated = 0;
            foreach (var actor in _actorContext)
            {
                Assert.NotNull(actor);
                iterated++;
            }
            
            Assert.Equal(_actorContext.Length, iterated);
        }
        
        [Fact]
        public void Filter()
        {
            var actorWithHealth = BuildActor(0);
            var actorWithoutHealth = BuildActor<ManaCostComponent>(1);

            _actorContext.Add(actorWithHealth);
            _actorContext.Add(actorWithoutHealth);
            
            var filter = _actorContext.GetFilter<HealthComponent>();

            Assert.True(filter.Contains(actorWithHealth));
            Assert.False(filter.Contains(actorWithoutHealth));
        }
        
        [Fact]
        public void Filter_Cache()
        {
            var filter1 = _actorContext.GetFilter<HealthComponent, ManaCostComponent>();
            var filter2 = _actorContext.GetFilter<HealthComponent, ManaCostComponent>();
            
            Assert.Same(filter1, filter2);
        }
        
        [Fact]
        public void Filter_TwoComponents()
        {
            var actorWithHealth = BuildActor(0);
            var actorWithoutHealth = BuildActor<ManaCostComponent>(1);

            _actorContext.Add(actorWithHealth);
            _actorContext.Add(actorWithoutHealth);
            
            var filter = _actorContext.GetFilter<HealthComponent, ManaCostComponent>();

            Assert.True(filter.Contains(actorWithHealth));
            Assert.False(filter.Contains(actorWithoutHealth));
        }

        [Fact]
        public void Group()
        {
            var creature = new Creature(1, BuildComponents());
            var notCreature = BuildActor(0);
            
            _actorContext.Add(creature);
            _actorContext.Add(notCreature);
            
            var actorGroup = _actorContext.GetGroup<Creature>();

            Assert.True(actorGroup.Contains(creature));
            Assert.Equal(1, actorGroup.Length);
        }
        
        [Fact]
        public void Group_Cache()
        {
            var group1 = _actorContext.GetGroup<Creature>();
            var group2 = _actorContext.GetGroup<Creature>();
            
            Assert.Same(group1, group2);
        }
        
        [Fact]
        public void Remove()
        {
            var actor = BuildActor(0);
            
            _actorContext.Add(actor);

            Measure(() => _actorContext.Remove(actor));

            Assert.False(_actorContext.Contains(actor));
        }

        [Fact]
        public void Remove_EventRaised()
        {
            var actor = BuildActor(0);
            
            _actorContext.Add(actor);

            _actorContext.Removed += added => Assert.Equal(actor, added);
            Measure(() => _actorContext.Remove(actor));
        }

        [Fact]
        public void Remove_FilterNotContains()
        {
            var actor = BuildActor(0);
            
            _actorContext.Add(actor);

            var filterManaCost = _actorContext.GetFilter<ManaCostComponent>();
            var filterHealth = _actorContext.GetFilter<HealthComponent>();
            var filter1 = _actorContext.GetFilter<HealthComponent, ManaCostComponent>();
            var filter2 = _actorContext.GetFilter<ManaCostComponent, HealthComponent>();

            Measure(() => _actorContext.Remove(actor));

            Assert.False(filterManaCost.Contains(actor));
            Assert.False(filterHealth.Contains(actor));
            Assert.False(filter1.Contains(actor));
            Assert.False(filter2.Contains(actor));
        }

        [Fact]
        public void Remove_GroupNotContains()
        {
            var actor = new Creature(0, BuildComponents());
            
            _actorContext.Add(actor);

            var creatures = _actorContext.GetGroup<Creature>();

            Measure(() => _actorContext.Remove(actor));

            Assert.False(creatures.Contains(actor));
        }
        
        [Fact]
        public void Where()
        {
            foreach (var actor in _actorContext.Where(a => a.Id >= 0))
            {
                Assert.True(actor.Id >= 0);
            }
        }
    }
}