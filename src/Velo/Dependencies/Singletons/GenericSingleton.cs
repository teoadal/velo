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

        public bool Applicable(Type requestedType)
        {
            return requestedType.IsGenericType && requestedType.GetGenericTypeDefinition() == _genericType;
        }

        public object Resolve(Type requestedType, DependencyContainer container)
        {
            if (_instances.TryGetValue(requestedType, out var existsInstance))
            {
                return existsInstance;
            }

            var instance = container.Activate(requestedType);
            _instances.Add(requestedType, instance);

            return instance;
        }
    }
}