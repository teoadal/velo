using Velo.ECS;
using Velo.ECS.Actors;

namespace Velo.TestsModels.ECS.Actors
{
    public sealed class Spell : Actor
    {
        public ManaCostComponent ManaCost => GetComponent<ManaCostComponent>();
        
        public Spell(int id, IComponent[] components) : base(id, components)
        {
        }
    }
}