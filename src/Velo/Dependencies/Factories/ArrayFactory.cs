using System;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Dependencies.Factories
{
    internal sealed class ArrayFactory : IDependency
    {
        private readonly IDependency[] _dependencies;

        public ArrayFactory(List<IDependency> dependencies)
        {
            _dependencies = dependencies.ToArray();
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
            if (elementType == null) throw Error.InvalidData($"Invalid array type {contract}");

            var elements = new List<object>();

            var dependencies = _dependencies;
            
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < dependencies.Length; i++)
            {
                var dependency = dependencies[i];
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
    }
}