using System;
using System.Reflection;
using Velo.Utils;

namespace Velo.Dependencies.Singletons
{
    internal sealed class ActivatorSingleton : Dependency
    {
        private readonly ConstructorInfo _constructor;
        private readonly Type _implementation;
        private readonly bool _isDisposable;

        private object _instance;

        public ActivatorSingleton(Type[] contracts, Type implementation) : base(contracts)
        {
            _implementation = implementation;

            _constructor = ReflectionUtils.GetConstructor(implementation);
            _isDisposable = _implementation.IsAssignableFrom(typeof(IDisposable));
        }

        public override void Destroy()
        {
            if (_isDisposable)
            {
                ((IDisposable) _instance)?.Dispose();
            }

            _instance = null;
        }

        public override object Resolve(Type contract, DependencyContainer container)
        {
            if (_instance != null) return _instance;

            _instance = container.Activate(_implementation, _constructor);
            return _instance;
        }
    }
}