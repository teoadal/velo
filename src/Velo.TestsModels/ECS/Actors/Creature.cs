using Velo.ECS;
using Velo.ECS.Actors;

namespace Velo.TestsModels.ECS.Actors
{
    public sealed class Creature : Actor
    {
        public HealthComponent Health => GetComponent<HealthComponent>();

        public ManaCostComponent ManaCost => GetComponent<ManaCostComponent>();

        public Creature(int id, IComponent[] components) : base(id, components)
        {
        }
    }
}