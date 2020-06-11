using System;
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

            public override void Init(DependencyLifetime lifetime, IDependencyEngine engine)
            {
                foreach (var dependency in _dependencies)
                {
                    if (lifetime == DependencyLifetime.Singleton && dependency.Lifetime == DependencyLifetime.Scoped)
                    {
                        throw Error.InconsistentLifetime(
                            _elementType.MakeArrayType(),
                            lifetime,
                            dependency.Implementation,
                            dependency.Lifetime);
                    }
                }
            }

            protected override object ResolveInstance(Type contract, IServiceProvider services)
            {
                var array = new T[_dependencies.Length];
                for (var index = 0; index < _dependencies.Length; index++)
                {
                    var dependency = _dependencies[index];
                    var instance = dependency.GetInstance(_elementType, services);
                    array[index] = (T) instance;
                }

                return array;
            }
        }
    }
}