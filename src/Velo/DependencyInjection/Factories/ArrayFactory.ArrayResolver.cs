using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.DependencyInjection.Factories
{
    internal sealed partial class ArrayFactory
    {
        private sealed class ArrayResolver<T> : DependencyResolver
        {
            private readonly IDependency[] _dependencies;
            private readonly Type _elementType;

            public ArrayResolver(IDependency[] dependencies) : base(Typeof<T[]>.Raw)
            {
                Array.Sort(dependencies, DependencyOrderComparer.Default);

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