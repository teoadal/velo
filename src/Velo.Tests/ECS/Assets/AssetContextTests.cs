using Velo.TestsModels.ECS;
using Velo.TestsModels.ECS.Assets;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS.Assets
{
    public class AssetContextTests : EcsTestClass
    {
        private readonly AssetContext _assetContext;
        
        public AssetContextTests(ITestOutputHelper output) : base(output)
        {
            _assetContext = new AssetContext();
        }

        [Fact]
        public void Add()
        {
            var asset = BuildAsset(0);
            
            Measure(() => _assetContext.Add(asset));

            Assert.True(_assetContext.Contains(asset));
        }

        [Fact]
        public void Add_EventRaised()
        {
            var asset = BuildAsset(0);
            
            _assetContext.Added += added => Assert.Equal(asset, added);
            
            Measure(() => _assetContext.Add(asset));
        }

        [Fact]
        public void Add_FilterContains_CreatedBefore()
        {
            var asset = BuildAsset(0);
            
            var filterManaCost = _assetContext.GetFilter<ManaCostComponent>();
            var filterHealth = _assetContext.GetFilter<HealthComponent>();
            var filter1 = _assetContext.GetFilter<HealthComponent, ManaCostComponent>();
            var filter2 = _assetContext.GetFilter<ManaCostComponent, HealthComponent>();

            Measure(() => _assetContext.Add(asset));

            Assert.True(filterManaCost.Contains(asset));
            Assert.True(filterHealth.Contains(asset));
            Assert.True(filter1.Contains(asset));
            Assert.True(filter2.Contains(asset));
        }

        [Fact]
        public void Add_FilterContains_CreatedAfter()
        {
            var asset = BuildAsset(0);
            
            Measure(() => _assetContext.Add(asset));

            var filterManaCost = _assetContext.GetFilter<ManaCostComponent>();
            var filterHealth = _assetContext.GetFilter<HealthComponent>();
            var filter1 = _assetContext.GetFilter<HealthComponent, ManaCostComponent>();
            var filter2 = _assetContext.GetFilter<ManaCostComponent, HealthComponent>();

            Assert.True(filterManaCost.Contains(asset));
            Assert.True(filterHealth.Contains(asset));
            Assert.True(filter1.Contains(asset));
            Assert.True(filter2.Contains(asset));
        }

        [Fact]
        public void Add_GroupContains_CreatedBefore()
        {
            var asset = new CreatureAsset(0, BuildComponent<ParametersComponent>());
            
            var creatures = _assetContext.GetGroup<CreatureAsset>();

            Measure(() => _assetContext.Add(asset));

            Assert.True(creatures.Contains(asset));
        }

        [Fact]
        public void Add_GroupContains_CreatedAfter()
        {
            var asset = new CreatureAsset(0, BuildComponent<ParametersComponent>());
            
            Measure(() => _assetContext.Add(asset));

            var creatures = _assetContext.GetGroup<CreatureAsset>();
            Assert.True(creatures.Contains(asset));
        }

        [Fact]
        public void Filter()
        {
            var assetWithHealth = BuildAsset(0);
            var assetWithoutHealth = BuildAsset<ManaCostComponent>(1);

            _assetContext.Add(assetWithHealth);
            _assetContext.Add(assetWithoutHealth);
            
            var filter = _assetContext.GetFilter<HealthComponent>();

            Assert.True(filter.Contains(assetWithHealth));
            Assert.False(filter.Contains(assetWithoutHealth));
        }
        
        [Fact]
        public void Filter_Cache()
        {
            var filter1 = _assetContext.GetFilter<HealthComponent, ManaCostComponent>();
            var filter2 = _assetContext.GetFilter<HealthComponent, ManaCostComponent>();
            
            Assert.Same(filter1, filter2);
        }
        
        [Fact]
        public void Filter_TwoComponents()
        {
            var assetWithHealth = BuildAsset(0);
            var assetWithoutHealth = BuildAsset<ManaCostComponent>(1);

            _assetContext.Add(assetWithHealth);
            _assetContext.Add(assetWithoutHealth);
            
            var filter = _assetContext.GetFilter<HealthComponent, ManaCostComponent>();

            Assert.True(filter.Contains(assetWithHealth));
            Assert.False(filter.Contains(assetWithoutHealth));
        }
        
        [Fact]
        public void Group()
        {
            var creatureAsset = new CreatureAsset(1, BuildComponent<ParametersComponent>());
            var notCreatureAsset = BuildAsset(0);
            
            _assetContext.Add(creatureAsset);
            _assetContext.Add(notCreatureAsset);
            
            var assetGroup = _assetContext.GetGroup<CreatureAsset>();

            Assert.True(assetGroup.Contains(creatureAsset));
            Assert.Equal(1, assetGroup.Length);
        }
        
        [Fact]
        public void Group_Cache()
        {
            var group1 = _assetContext.GetGroup<CreatureAsset>();
            var group2 = _assetContext.GetGroup<CreatureAsset>();
            
            Assert.Same(group1, group2);
        }
    }
}