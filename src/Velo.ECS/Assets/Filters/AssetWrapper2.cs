using Velo.ECS.Components;

namespace Velo.ECS.Assets.Filters
{
    public readonly struct Asset<TComponent1, TComponent2>
        where TComponent1 : IComponent where TComponent2 : IComponent
    {
        public int Id => Entity.Id;

        public readonly TComponent1 Component1;

        public readonly TComponent2 Component2;

        public readonly Asset Entity;

        public Asset(Asset entity, TComponent1 component1, TComponent2 component2)
        {
            Entity = entity;
            Component1 = component1;
            Component2 = component2;
        }
    }
}