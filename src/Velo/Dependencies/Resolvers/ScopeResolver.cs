using System;
using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    internal sealed class ScopeResolver : DependencyResolver
    {
        private readonly IDependency _dependency;
        private readonly int _hash;

        public ScopeResolver(IDependency dependency, string dependencyName = null)
            : base(dependency, dependencyName)
        {
            _dependency = dependency;

            unchecked
            {
                _hash = dependency.GetHashCode();
                _hash ^= dependencyName?.GetHashCode() ?? 1;
            }
        }

        public override object Resolve(Type contract, DependencyContainer container)
        {
            var currentScope = DependencyScope.Current;
            if (currentScope == null) throw Error.InvalidOperation("Scope is not started");

            if (currentScope.TryGetInstance(this, out var existsInstance))
            {
                return existsInstance;
            }

            var instance = _dependency.Resolve(contract, container);
            currentScope.Add(this, instance);

            return instance;
        }

        public override bool Equals(object obj)
        {
            return obj is ScopeResolver other && other._hash == _hash;
        }

        public override int GetHashCode() => _hash;
    }
}