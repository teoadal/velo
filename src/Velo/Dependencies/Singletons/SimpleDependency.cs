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
            _contract = contract;
            _implementation = implementation;

            _constructor = ReflectionUtils.GetConstructor(implementation);
            _isDisposable = ReflectionUtils.IsDisposableType(implementation);
        }

        public bool Applicable(Type contract)
        {
            return _contract == contract;
        }

        public void Destroy()
        {
            if (_isDisposable)
            {
                ((IDisposable) _instance)?.Dispose();
            }

            _instance = null;
        }

        public object Resolve(Type contract, DependencyContainer container)
        {
            if (_instance != null) return _instance;

            _instance = container.Activate(_implementation, _constructor);
            return _instance;
        }
        
        public override string ToString()
        {
            return $"Dependency for {_contract.Name}";
        }
    }
}