using System;

namespace Velo.ECS.Components
{
    public interface IComponentFactory
    {
        IComponent Create(Type componentType);

        TComponent Create<TComponent>() where TComponent : IComponent;
    }
}