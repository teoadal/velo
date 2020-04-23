using Velo.ECS.Components;

namespace Velo.ECS.Actors.Filters
{
    public readonly struct Actor<TComponent>
        where TComponent : IComponent
    {
        public int Id => Entity.Id;

        public readonly TComponent Component1;

        public readonly Actor Entity;

        public Actor(Actor actor, TComponent component1)
        {
            Entity = actor;
            Component1 = component1;
        }

        public static implicit operator Actor(Actor<TComponent> wrapper)
        {
            return wrapper.Entity;
        }
    }
}