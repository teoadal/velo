using System;
using System.Collections.Generic;
using Velo.Dependencies.Resolvers;
using Velo.Utils;

namespace Velo.Dependencies.Factories
{
    internal sealed class ArrayFactory : IDependency
    {
        private readonly IDependencyResolver[] _resolvers;

        public ArrayFactory(List<IDependencyResolver> dependencies)
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
            if (elementType == null) throw Error.InvalidData($"Invalid array type {contract}");

            var elements = new List<object>();
            
            var resolvers = _resolvers;
            for (var i = 0; i < resolvers.Length; i++)
            {
                var dependency = resolvers[i];
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