using System;
using Velo.Utils;

namespace Velo.DependencyInjection.Scan
{
    internal sealed class GenericInterfaceAllover : IDependencyAllover
    {
        private readonly Type _genericContract;
        private readonly DependencyLifetime _lifetime;

        public GenericInterfaceAllover(Type genericContract, DependencyLifetime lifetime)
        {
            _genericContract = genericContract;
            _lifetime = lifetime;
        }

        public void TryRegister(DependencyCollection collection, Type implementation)
        {
            var interfaces = implementation.GetInterfaces();
            foreach (var interfaceType in interfaces)
            {
                if (!ReflectionUtils.IsGenericTypeImplementation(interfaceType, _genericContract)) continue;

                collection.AddDependency(interfaceType, implementation, _lifetime);
                return;
            }
        }
    }
}