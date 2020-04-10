using Velo.Tests.NewECS.Components;

namespace Velo.Tests.NewECS.Actors.Filters
{
    public readonly struct Actor<TComponent1, TComponent2>
        where TComponent1 : IComponent where TComponent2 : IComponent
    {
        public int Id => Entity.Id;

        public readonly TComponent1 Component1;

        public readonly TComponent2 Component2;

        public readonly Actor Entity;

        public Actor(Actor entity, TComponent1 component1, TComponent2 component2)
        {
            Entity = entity;
            Component1 = component1;
            Component2 = component2;
        }

        public static implicit operator Actor(Actor<TComponent1, TComponent2> wrapper)
        {
            return wrapper.Entity;
        }
    }
}