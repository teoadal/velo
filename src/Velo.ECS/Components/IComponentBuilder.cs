namespace Velo.ECS.Components
{
    public interface IComponentBuilder
    {
        IComponent BuildComponent();
    }

    public interface IComponentBuilder<out TComponent> : IComponentBuilder
        where TComponent : IComponent
    {
        TComponent Build();
    }

    internal sealed class DefaultComponentBuilder<TComponent> : IComponentBuilder<TComponent>
        where TComponent : IComponent, new()
    {
        public TComponent Build()
        {
            return new TComponent();
        }

        public IComponent BuildComponent() => Build();
    }
}