using System;
using System.Collections.Generic;
using System.IO;

namespace Velo.Dependencies.Factories
{
    internal sealed class ArrayFactory : IDependency
    {
        private readonly DependencyResolver[] _resolvers;

        public ArrayFactory(List<DependencyResolver> dependencies)
        {
            _resolvers = dependencies.ToArray();
        }

        public bool Applicable(Type contract)
        {
            return contract.IsArray;
        }

        public void Destroy()
        {
        }

        public object Resolve(Type contract, DependencyContainer container)
        {
            var elementType = contract.GetElementType();

            if (elementType == null)
            {
                throw new InvalidDataException($"Invalid array type {contract}");
            }

            var elements = new List<object>();
            for (var i = 0; i < _resolvers.Length; i++)
            {
                var dependency = _resolvers[i];
                if (!dependency.Applicable(elementType)) continue;

                var resolved = dependency.Resolve(elementType, container);
                elements.Add(resolved);
            }

            var array = Array.CreateInstance(elementType, elements.Count);
            for (var i = 0; i < elements.Count; i++)
            {
                array.SetValue(elements[i], i);
            }

            elements.Clear();
            return array;
        }
        
        public override string ToString()
        {
            return $"Dependency for Array";
        }
    }
}