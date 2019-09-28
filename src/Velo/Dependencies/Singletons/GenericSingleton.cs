using System;
using System.Collections.Generic;

namespace Velo.Dependencies.Singletons
{
    internal sealed class GenericSingleton : IDependency
    {
        private readonly Type _genericContract;
        private readonly Type _genericImplementation;

        private readonly Dictionary<Type, object> _instances;

        public GenericSingleton(Type genericContract, Type genericImplementation = null)
        {
            _genericContract = genericContract;
            _genericImplementation = genericImplementation;
            
            _instances = new Dictionary<Type, object>();
        }

        public bool Applicable(Type contract)
        {
            return contract.IsGenericType && contract.GetGenericTypeDefinition() == _genericContract;
        }

        public void Destroy()
        {
            foreach (var pair in _instances)
            {
                var instance = pair.Value;
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            
            _instances.Clear();
        }

        public object Resolve(Type contract, DependencyContainer container)
        {
            if (_instances.TryGetValue(contract, out var existsInstance))
            {
                return existsInstance;
            }

            var implementation = _genericImplementation == null
                ? contract
                : _genericImplementation.MakeGenericType(contract.GetGenericArguments());
            
            var instance = container.Activate(implementation);
            
            _instances.Add(contract, instance);

            return instance;
        }
    }
}