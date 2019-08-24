using System;
using System.Reflection;
using Velo.Utils;

namespace Velo.Dependencies.Singletons
{
    internal sealed class SimpleDependency : IDependency
    {
        private readonly ConstructorInfo _constructor;
        private readonly Type _contract;
        private readonly Type _implementation;
        private readonly bool _isDisposable;

        private object _instance;

        public SimpleDependency(Type contract, Type implementation)
        {
            _constructor = ReflectionUtils.GetConstructor(implementation);
            _contract = contract;
            _implementation = implementation;
            _isDisposable = _implementation.IsAssignableFrom(typeof(IDisposable));
        }

        public bool Applicable(Type requestedType)
        {
            return _contract == requestedType;
        }

        public void Destroy()
        {
            if (_isDisposable)
            {
                ((IDisposable) _instance)?.Dispose();
            }
        }

        public object Resolve(Type requestedType, DependencyContainer container)
        {
            if (_instance != null) return _instance;

            _instance = container.Activate(_implementation, _constructor);
            return _instance;
        }
        
    }
}