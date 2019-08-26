using System;

namespace Velo.Dependencies.Scan
{
    internal sealed class GenericInterfaceScanner : IDependencyScanner
    {
        private readonly Type _genericDefinition;
        private Type _applicableType;

        public GenericInterfaceScanner(Type genericDefinition)
        {
            _genericDefinition = genericDefinition;
        }

        public bool Applicable(Type type)
        {
            return TryGetGenericInterface(type, out _applicableType);
        }

        public void Register(DependencyBuilder builder, Type implementation)
        {
            // _applicableType variable is not null.
            // Register method should be called after Applicable method
            // and if Applicable method returns true.
            builder.AddSingleton(_applicableType, implementation);
            _applicableType = null;
        }

        private bool TryGetGenericInterface(Type type, out Type genericType)
        {
            var interfaces = type.GetInterfaces();
            for (var i = 0; i < interfaces.Length; i++)
            {
                var interfaceType = interfaces[i];
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _genericDefinition)
                {
                    genericType = interfaceType;
                    return true;
                }
            }

            genericType = null;
            return false;
        }
    }
}