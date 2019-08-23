using System;
using System.Collections.Generic;
using System.IO;

namespace Velo.Dependencies.Factories
{
    internal sealed class ArrayFactory : IDependency
    {
        private readonly IDependency[] _dependencies;

        public ArrayFactory(List<IDependency> dependencies)
        {
            _dependencies = dependencies.ToArray();
        }

        public bool Applicable(Type requestedType)
        {
            return requestedType.IsArray;
        }

        public void Destroy()
        {
        }

        public object Resolve(Type requestedType, DependencyContainer container)
        {
            var elementType = requestedType.GetElementType();

            if (elementType == null)
            {
                throw new InvalidDataException($"Invalid array type {requestedType}");
            }

            var elements = new List<object>();
            for (var i = 0; i < _dependencies.Length; i++)
            {
                var dependency = _dependencies[i];
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