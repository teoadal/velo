using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Velo.Collections;
using Velo.Collections.Enumerators;
using Velo.ECS.Components;
using Velo.Threading;

namespace Velo.ECS.Actors
{
    public class Actor : IEntity, IEquatable<Actor>
    {
        public readonly int Id;

        public event Action<Actor, IComponent>? ComponentAdded;

        public event Action<Actor, IComponent>? ComponentRemoved;

        private readonly ReaderWriterLockSlim _lock;

        private IComponent[] _components;
        private int _componentsLength;

        public Actor(int id, IComponent[]? components = null)
        {
            Id = id;

            _lock = new ReaderWriterLockSlim();

            _components = components ?? Array.Empty<IComponent>();
            _componentsLength = _components.Length;
        }

        public void AddComponent(IComponent component)
        {
            using (WriteLock.Enter(_lock))
            {
                CollectionUtils.Insert(ref _components, ref _componentsLength, component);
            }

            OnComponentAdded(component);
        }

        public void AddComponents(params IComponent[] components)
        {
            using (WriteLock.Enter(_lock))
            {
                CollectionUtils.EnsureCapacity(ref _components, _componentsLength + components.Length);

                foreach (var component in components)
                {
                    CollectionUtils.Insert(ref _components, ref _componentsLength, component);
                }
            }

            foreach (var component in components)
            {
                OnComponentAdded(component);
            }
        }

        public bool ContainsComponent<TComponent>() where TComponent : IComponent
        {
            return TryGetComponent<TComponent>(out _);
        }

        public bool ContainsComponents<TComponent1, TComponent2>()
            where TComponent1 : IComponent where TComponent2 : IComponent
        {
            using (ReadLock.Enter(_lock))
            {
                return _components.ContainsComponents<TComponent1, TComponent2>();
            }
        }

        #region Equals

        public bool Equals(Actor? other)
        {
            return other != null && other.Id == Id;
        }

        public override bool Equals(object? obj)
        {
            return obj != null && Equals(obj as Actor);
        }

        #endregion

        public override int GetHashCode() => Id;

        public bool RemoveComponent<TComponent>() where TComponent : IComponent
        {
            IComponent? removedComponent = null;
            using (WriteLock.Enter(_lock))
            {
                for (var i = 0; i < _components.Length; i++)
                {
                    if (i == _componentsLength) break;

                    var component = _components[i];
                    if (!(component is TComponent)) continue;

                    CollectionUtils.Cut(ref _components, i);
                    _componentsLength--;

                    removedComponent = component;
                    break;
                }
            }

            if (removedComponent == null) return false;
            OnComponentRemoved(removedComponent);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : IComponent
        {
            using (ReadLock.Enter(_lock))
            {
                foreach (var exists in _components)
                {
                    if (!(exists is TComponent result)) continue;

                    component = result;
                    return true;
                }
            }

            component = default!;
            return false;
        }

        public bool TryGetComponents<TComponent1, TComponent2>(out TComponent1 component1, out TComponent2 component2)
            where TComponent1 : IComponent where TComponent2 : IComponent
        {
            using (ReadLock.Enter(_lock))
            {
                return _components.TryGetComponents(out component1, out component2);
            }
        }

        protected virtual void OnComponentAdded(IComponent component)
        {
            var evt = ComponentAdded;
            evt?.Invoke(this, component);
        }

        protected virtual void OnComponentRemoved(IComponent component)
        {
            var evt = ComponentRemoved;
            evt?.Invoke(this, component);
        }

        int IEntity.Id => Id;
        IEnumerable<IComponent> IEntity.Components => new ArrayLockEnumerator<IComponent>(_components, _lock);
    }
}