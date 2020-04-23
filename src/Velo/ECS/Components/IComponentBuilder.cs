namespace Velo.ECS.Components
{
    public interface IComponentBuilder
    {
    }

    public interface IComponentBuilder<out TComponent> : IComponentBuilder
        where TComponent : IComponent
    {
        TComponent Build();
    }
}