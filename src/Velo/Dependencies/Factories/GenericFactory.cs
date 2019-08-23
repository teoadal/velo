using System;

namespace Velo.Dependencies.Factories
{
    internal sealed class GenericFactory : IDependency
    {
        private readonly Type _genericType;

        public GenericFactory(Type genericType)
        {
            _genericType = genericType;
        }

        public bool Applicable(Type requestedType)
        {
            return requestedType.IsGenericType && requestedType.GetGenericTypeDefinition() == _genericType;
        }

        public void Destroy()
        {
        }

        public object Resolve(Type requestedType, DependencyContainer container)
        {
            return container.Activate(requestedType);
        }
    }
}