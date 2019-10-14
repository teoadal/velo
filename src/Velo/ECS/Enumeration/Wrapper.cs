namespace Velo.ECS.Enumeration
{
    public readonly struct Wrapper<TEntity, TComponent1>
        where TEntity: Entity where TComponent1: IComponent
    {
        public readonly TEntity Entity;

        public readonly TComponent1 Component1;

        public Wrapper(TEntity entity, TComponent1 component1)
        {
            Entity = entity;
            Component1 = component1;
        }
    }

    public readonly struct Wrapper<TEntity, TComponent1, TComponent2>
        where TEntity: Entity where TComponent1: IComponent where TComponent2: IComponent
    {
        public readonly TEntity Entity;

        public readonly TComponent1 Component1;
        
        public readonly TComponent2 Component2;

        public Wrapper(TEntity entity, TComponent1 component1, TComponent2 component2)
        {
            Entity = entity;
            Component1 = component1;
            Component2 = component2;
        }
    }
}