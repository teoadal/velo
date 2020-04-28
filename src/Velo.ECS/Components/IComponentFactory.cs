namespace Velo.ECS.Components
{
    public interface IComponentFactory
    {
        TComponent Create<TComponent>() where TComponent : IComponent;
    }
}