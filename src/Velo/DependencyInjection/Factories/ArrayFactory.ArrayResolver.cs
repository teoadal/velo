using System;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;

namespace Velo.DependencyInjection.Factories
{
    internal sealed partial class ArrayFactory
    {
        private class ArrayResolver<T>: DependencyResolver
        {
            private readonly IDependency[] _dependencies;
            private readonly Type _elementType;

            public ArrayResolver(IDependency[] dependencies)
            {
                _dependencies = dependencies;
                _elementType = typeof(T);
            }

            protected override object GetInstance(Type contract, IDependencyScope scope)
            {
                var array = new T[_dependencies.Length];

                for (var index = 0; index < _dependencies.Length; index++)
                {
                    var dependency = _dependencies[index];
                    var instance = dependency.GetInstance(_elementType, scope);
                    array[index] = (T) instance;
                }

                return array;
            }
        }
    }
}