using Velo.ECS.Actors;
using Velo.ECS.Components;

namespace Velo.TestsModels.ECS
{
    public sealed class TestActor : Actor
    {
        public TestActor(int id, IComponent[] components = null) : base(id, components)
        {
        }
    }
}