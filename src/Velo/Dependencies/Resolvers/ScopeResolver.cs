using System;
using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    internal sealed class ScopeResolver : DependencyResolver
    {
        private readonly IDependency _dependency;
        private readonly int _hash;
        private readonly Func<IDependency, ResolveContext, object> _resolveInstance;

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

            var context = new ResolveContext(_dependency, contract, container);
            return currentScope.GetOrAdd(this, _resolveInstance, context);
        }

        private static object ResolveInstance(IDependency dependency, ResolveContext context)
        {
            return context.Dependency.Resolve(context.Contract, context.Container);
        }

        public override bool Equals(object obj)
        {
            return obj is ScopeResolver other && other._hash == _hash;
        }

        public override int GetHashCode() => _hash;

        private readonly struct ResolveContext
        {
            public readonly IDependency Dependency;
            public readonly Type Contract;
            public readonly DependencyContainer Container;

            public ResolveContext(IDependency dependency, Type contract, DependencyContainer container)
            {
                Dependency = dependency;
                Contract = contract;
                Container = container;
            }
        }
    }
}