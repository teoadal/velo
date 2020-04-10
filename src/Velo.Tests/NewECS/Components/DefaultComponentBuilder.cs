namespace Velo.Tests.NewECS.Components
{
    internal sealed class DefaultComponentBuilder<TComponent> : IComponentBuilder<TComponent>
        where TComponent : IComponent, new()
    {
        public TComponent Build()
        {
            return new TComponent();
        }
    }
}