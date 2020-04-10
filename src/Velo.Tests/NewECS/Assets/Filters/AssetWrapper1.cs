using Velo.Tests.NewECS.Components;

namespace Velo.Tests.NewECS.Assets.Filters
{
    public readonly struct Asset<TComponent>
        where TComponent : IComponent
    {
        public int Id => Entity.Id;

        public readonly TComponent Component1;

        public readonly Asset Entity;

        public Asset(Asset entity, TComponent component1)
        {
            Entity = entity;
            Component1 = component1;
        }
    }
}