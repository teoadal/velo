using System;
using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    internal sealed class ScopeResolver : DependencyResolver
    {
        private readonly IDependency _dependency;
        private readonly int _hash;

        public ScopeResolver(IDependency dependency) : base(dependency)
        {
            _dependency = dependency;
            _hash = dependency.GetHashCode();
        }

        public override object Resolve(Type contract, DependencyContainer container)
        {
            var currentScope = DependencyScope.Current;
            if (currentScope == null) throw Error.InvalidOperation("Scope is not started");

            if (currentScope.TryGetInstance(this, out var existsInstance))
            {
                return existsInstance;
            }

            currentScope.BeginResolving(this);

            var instance = _dependency.Resolve(contract, container);
            currentScope.Add(this, instance);

            currentScope.ResolvingComplete(this);

            return instance;
        }

        public override bool Equals(object obj)
        {
            return obj is ScopeResolver other && other._hash == _hash;
        }

        public override int GetHashCode() => _hash;
    }
}