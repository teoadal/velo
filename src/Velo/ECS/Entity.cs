using System;
using Velo.Utils;

namespace Velo.ECS
{
    public class Entity : IEquatable<Entity>
    {
        public readonly int Id;

        public ref Sign Sign => ref _sign;

        // ReSharper disable InconsistentNaming
        protected IComponent[] _components;
        protected Sign _sign;
        // ReSharper restore InconsistentNaming

        internal Entity(int id, IComponent[] components)
        {
            Id = id;

            _components = components;
            _sign = SignBuilder.Create(components);
        }

        public bool Contains<TComponent>() where TComponent : IComponent
        {
            return _sign.Contains(Typeof<TComponent>.Id);
        }

        public TComponent Get<TComponent>() where TComponent : IComponent
        {
            var typeId = Typeof<TComponent>.Id;
            var index = _sign.IndexOf(typeId);

            return (TComponent) _components[index];
        }

        public bool Equals(Entity other)
        {
            return other != null && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}