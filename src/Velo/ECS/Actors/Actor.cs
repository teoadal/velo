using System;
using Velo.Utils;

namespace Velo.ECS.Actors
{
    public class Actor : Entity
    {
        public event Action<Actor, IComponent> AddedComponent;

        public event Action<Actor, IComponent> RemovedComponent;

        internal Actor(int id, IComponent[] components) : base(id, components)
        {
        }

        public void AddComponent<TComponent>(TComponent component) where TComponent : class, IComponent
        {
            var typeId = Typeof<TComponent>.Id;
            _sign = _sign.Add(typeId, out var index);

            Array.Resize(ref _components, _components.Length + 1);
            _components[index] = component;

            OnComponentAdded(component);
        }

        public void RemoveComponent<TComponent>() where TComponent : IComponent
        {
            var typeId = Typeof<TComponent>.Id;
            _sign = _sign.Remove(typeId, out var index);

            var component = _components[index];
            _components[index] = null;

            OnComponentRemoved(component);

            component.Dispose();
        }

        private void OnComponentAdded(IComponent component)
        {
            var evt = AddedComponent;
            evt?.Invoke(this, component);
        }

        private void OnComponentRemoved(IComponent component)
        {
            var evt = RemovedComponent;
            evt?.Invoke(this, component);
        }
    }
}