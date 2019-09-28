using System;
using System.Collections.Generic;

namespace Velo.Dependencies.Singletons
{
    internal sealed class GenericSingleton : GenericDependency
    {
        private readonly Type _genericImplementation;

        private readonly Dictionary<Type, object> _instances;

        public GenericSingleton(Type[] genericContracts, Type genericImplementation = null) : base(genericContracts)
        {
            _genericImplementation = genericImplementation;
            
            _instances = new Dictionary<Type, object>();
        }

        public override void Destroy()
        {
            foreach (var instance in _instances.Values)
            {
                if (instance is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            
            _instances.Clear();
        }

        public override object Resolve(Type contract, DependencyContainer container)
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