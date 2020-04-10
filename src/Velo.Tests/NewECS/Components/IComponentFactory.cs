namespace Velo.Tests.NewECS.Components
{
    public interface IComponentFactory
    {
        TComponent Create<TComponent>() where TComponent : IComponent;
    }
}