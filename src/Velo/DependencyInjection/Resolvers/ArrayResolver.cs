using System;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Engine;

namespace Velo.DependencyInjection.Resolvers
{
    internal sealed class ArrayResolver : DependencyResolver
    {
        public readonly Dependency[] Dependencies;

        private readonly Type _elementType;

        public ArrayResolver(Type elementType, Dependency[] dependencies)
            : base(elementType.MakeArrayType(), DependencyLifetime.Transient)
        {
            Dependencies = DependencyComparer.Sort(dependencies);

            _elementType = elementType;
        }

        public ArrayResolver(Type elementType, Dependency dependency)
            : this(elementType, new[] {dependency})
        {
        }

        public override object Resolve(DependencyProvider scope)
        {
            var array = Array.CreateInstance(_elementType, Dependencies.Length);

            for (var i = 0; i < Dependencies.Length; i++)
            {
                var instance = Dependencies[i].GetInstance(scope);
                array.SetValue(instance, i);
            }

            return array;
        }

        protected override void Initialize(DependencyEngine engine)
        {
        }
    }
}