using System;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Engines;
using Velo.Utils;

namespace Velo.DependencyInjection.Resolvers
{
    internal sealed class ArrayResolver : DependencyResolver
    {
        public Dependency[] Dependencies => _dependencies;

        private Dependency[] _dependencies;
        private Type _elementType;

        public ArrayResolver(Type elementType, Dependency[] dependencies)
            : base(elementType.MakeArrayType(), DependencyLifetime.Transient)
        {
            _dependencies = DependencyComparer.Sort(dependencies);

            _elementType = elementType;
        }

        public ArrayResolver(Type elementType, Dependency dependency)
            : this(elementType, new[] {dependency})
        {
        }

        public override object Resolve(DependencyProvider scope)
        {
            var dependencies = _dependencies;
            
            var array = Array.CreateInstance(_elementType, dependencies.Length);
            for (var i = 0; i < dependencies.Length; i++)
            {
                var instance = dependencies[i].GetInstance(scope);
                array.SetValue(instance, i);
            }

            return array;
        }

        protected override void Initialize(DependencyEngine engine)
        {
        }

        public override void Dispose()
        {
            CollectionUtils.DisposeValuesIfDisposable(_dependencies);
            _dependencies = null;
            _elementType = null;
        }
    }
}