using System.Runtime.CompilerServices;
using Velo.ECS.Components;

namespace Velo.ECS.Assets
{
    public class Asset
    {
        public readonly int Id;

        private readonly IComponent[] _components;

        public Asset(int id, IComponent[] components)
        {
            Id = id;
            _components = components;
        }

        public bool ContainsComponent<TComponent>() where TComponent : IComponent
        {
            return TryGetComponent<TComponent>(out _);
        }

        public bool ContainsComponents<TComponent1, TComponent2>()
            where TComponent1 : IComponent where TComponent2 : IComponent
        {
            return _components.ContainsComponents<TComponent1, TComponent2>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : IComponent
        {
            foreach (var exists in _components)
            {
                if (!(exists is TComponent result)) continue;

                component = result;
                return true;
            }

            component = default!;
            return false;
        }

        public bool TryGetComponents<TComponent1, TComponent2>(out TComponent1 component1, out TComponent2 component2)
            where TComponent1 : IComponent where TComponent2 : IComponent
        {
            return _components.TryGetComponents(out component1, out component2);
        }
    }
}