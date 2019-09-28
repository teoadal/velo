using Velo.Utils;

namespace Velo.Dependencies.Resolvers
{
    internal sealed class ScopeDependencyResolver : DependencyResolver
    {
        public ScopeDependencyResolver(IDependency dependency, string dependencyName = null)
            : base(dependency, dependencyName)
        {
        }

        protected override void ResolveComplete(object resolvedInstance, DependencyContainer container)
        {
            var currentScope = DependencyScope.Current;
            if (currentScope == null) throw Error.InvalidOperation("Scope is not started");
            
            if (!currentScope.TryAdd(this)) throw Error.InvalidOperation($"{this} already added in scope");
        }
    }
}