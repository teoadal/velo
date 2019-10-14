using System;
using Velo.Utils;

namespace Velo.DependencyInjection.Scan
{
    internal sealed class GenericInterfaceAllover : DependencyAllover
    {
        private readonly Type _genericContract;
        private readonly DependencyLifetime _lifetime;

        public GenericInterfaceAllover(Type genericContract, DependencyLifetime lifetime)
        {
            _genericContract = genericContract;
            _lifetime = lifetime;
        }

        public override void TryRegister(DependencyCollection collection, Type implementation)
        {
            var interfaces = implementation.GetInterfaces();
            for (var i = 0; i < interfaces.Length; i++)
            {
                var interfaceType = interfaces[i];
                if (!ReflectionUtils.IsGenericTypeImplementation(interfaceType, _genericContract)) continue;
                
                collection.Add(interfaceType, implementation, _lifetime);
                return;
            }
        }
    }
}