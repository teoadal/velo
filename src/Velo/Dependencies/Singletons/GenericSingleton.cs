using System;
using System.Collections.Generic;

namespace Velo.Dependencies.Singletons
{
    internal sealed class GenericSingleton : IDependency
    {
        private readonly Type _genericType;
        
        private readonly Dictionary<Type, object> _instances;

        public GenericSingleton(Type genericType)
        {
            _genericType = genericType;
            _instances = new Dictionary<Type, object>();
        }

        public bool Applicable(Type contract)
        {
            return contract.IsGenericType && contract.GetGenericTypeDefinition() == _genericType;
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

            var instance = container.Activate(contract);
            _instances.Add(contract, instance);

            return instance;
        }
    }
}