using Velo.Tests.NewECS.Actors;
using Velo.Tests.NewECS.Components;

namespace Velo.Tests.NewECS.Tests
{
    public sealed class TestActor : Actor
    {
        public TestActor(int id, IComponent[] components = null) : base(id, components)
        {
        }
    }
}