using System;
using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    internal sealed class ScopeResolver : DependencyResolver
    {
        private readonly IDependency _dependency;
        private readonly int _hash;
        private readonly Func<IDependency, Type, DependencyContainer, object> _resolveInstance;

        public ScopeResolver(IDependency dependency) : base(dependency)
        {
            _dependency = dependency;
            _hash = dependency.GetHashCode();
            _resolveInstance = ResolveInstance;
        }

        public override object Resolve(Type contract, DependencyContainer container)
        {
            var currentScope = DependencyScope.Current;
            if (currentScope == null) throw Error.InvalidOperation("Scope is not started");

            return currentScope.GetOrAdd(this, _dependency, contract, container, _resolveInstance);
        }

        private static object ResolveInstance(IDependency dependency, Type contract, DependencyContainer container)
        {
            return dependency.Resolve(contract, container);
        }

        public override bool Equals(object obj)
        {
            return obj is ScopeResolver other && other._hash == _hash;
        }

        public override int GetHashCode() => _hash;
    }
}