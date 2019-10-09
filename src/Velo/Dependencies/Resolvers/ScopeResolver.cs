using System;
using System.Diagnostics;
using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    [DebuggerDisplay("Scope {" + nameof(_dependency) + "}")]
    internal sealed class ScopeResolver : DependencyResolver
    {
        private readonly IDependency _dependency;

        public ScopeResolver(IDependency dependency) : base(dependency)
        {
            _dependency = dependency;
        }

        public override object Resolve(Type contract, DependencyContainer container)
        {
            var currentScope = DependencyScope.Current;
            if (currentScope == null) throw Error.InvalidOperation("Scope is not started");

            return currentScope.GetOrAdd(_dependency, contract);
        }
    }
}