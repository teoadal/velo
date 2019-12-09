using System;
using Velo.Utils;

namespace Velo.ECS.Actors
{
    public class Actor : Entity
    {
        public event Action<Actor, IComponent> AddedComponent;

        public event Action<Actor, IComponent> RemovedComponent;

        public Actor(int id, IComponent[] components) : base(id, components)
        {
        }

        public void AddComponent<TComponent>(TComponent component) where TComponent : class, IComponent
        {
            var typeId = Typeof<TComponent>.Id;

            if (_sign.Contains(typeId))
            {
                throw Error.InvalidOperation($"Component {ReflectionUtils.GetName<TComponent>()} already exists");
            }
            
            _sign = _sign.Add(typeId, out var index);
            Array.Resize(ref _components, _components.Length + 1);
            _components[index] = component;

            OnComponentAdded(component);
        }

        public bool RemoveComponent<TComponent>() where TComponent : IComponent
        {
            if (!_sign.TryRemove(Typeof<TComponent>.Id, out var newSign, out var index))
            {
                return false;
            }
            
            _sign = newSign;

            var component = _components[index];
            CollectionUtils.Cut(ref _components, index);

            OnComponentRemoved(component);
            component.Dispose();

            return true;
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