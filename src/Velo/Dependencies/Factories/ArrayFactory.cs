using System;
using System.Collections.Concurrent;

namespace Velo.Dependencies.Factories
{
    internal sealed class ArrayFactory : IDependency
    {
        private readonly ConcurrentDictionary<Type, IDependency[]> _arrayDependencies;

        public ArrayFactory()
        {
            _arrayDependencies = new ConcurrentDictionary<Type, IDependency[]>();
        }

        public bool Applicable(Type contract)
        {
            return contract.IsArray;
        }

        public void Destroy()
        {
        }

        public void Init(DependencyContainer container)
        {
        }

        public object Resolve(Type contract, DependencyContainer container)
        {
            var elementType = contract.GetElementType();

            // ReSharper disable once AssignNullToNotNullAttribute
            var arrayDependencies = _arrayDependencies.GetOrAdd(elementType, container.GetDependencies);

            var array = Array.CreateInstance(elementType, arrayDependencies.Length);
            for (var i = 0; i < arrayDependencies.Length; i++)
            {
                var dependency = arrayDependencies[i];
                array.SetValue(dependency.Resolve(elementType, container), i);
            }

            return array;
        }
    }
}