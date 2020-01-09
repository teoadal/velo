using System.Threading.Tasks;
using Velo.DependencyInjection;
using Velo.TestsModels.ECS;
using Xunit;
using Xunit.Abstractions;

namespace Velo.ECS
{
    public class WorldTests : EcsTestBase
    {
        private readonly DependencyProvider _provider;

        public WorldTests(ITestOutputHelper output) : base(output)
        {
            _provider = new DependencyCollection()
                .AddECS()
                .AddECSSystem<ComplexSystem>()
                .BuildProvider();
        }

        [Fact]
        public async Task Available()
        {
            var world = _provider.GetRequiredService<World>();

            await world.Init();
            await world.Update();
        }

        [Fact]
        public async Task SystemsCall()
        {
            var world = _provider.GetRequiredService<World>();
            var system = _provider.GetRequiredService<ComplexSystem>();

            await world.Init();

            Assert.Equal(1, system.InitStep);

            await world.Update();

            Assert.Equal(2, system.BeforeStep);
            Assert.Equal(3, system.UpdateStep);
            Assert.Equal(4, system.AfterStep);
        }
    }
}