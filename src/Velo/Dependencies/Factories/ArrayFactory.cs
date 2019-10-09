using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Dependencies.Factories
{
    internal sealed class ArrayFactory : IDependency
    {
        private readonly ConcurrentDictionary<Type, IDependency[]> _arrayDependencies;
        private readonly IDependency[] _dependencies;
        private readonly Func<Type, IDependency[]> _findDependencies;

        public ArrayFactory(List<IDependency> dependencies)
        {
            _arrayDependencies = new ConcurrentDictionary<Type, IDependency[]>();
            _dependencies = dependencies.ToArray();
            _findDependencies = FindDependencies;
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

            var arrayDependencies = _arrayDependencies.GetOrAdd(elementType, _findDependencies);

            var array = Array.CreateInstance(elementType, arrayDependencies.Length);
            for (var i = 0; i < arrayDependencies.Length; i++)
            {
                var dependency = arrayDependencies[i];
                array.SetValue(dependency.Resolve(elementType, container), i);
            }

            return array;
        }

        private IDependency[] FindDependencies(Type elementType)
        {
            var dependencies = _dependencies;
            var arrayDependencies = new List<IDependency>();

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < dependencies.Length; i++)
            {
                var dependency = dependencies[i];
                if (dependency.Applicable(elementType))
                {
                    arrayDependencies.Add(dependency);
                }
            }

            return arrayDependencies.ToArray();
        }
    }
}