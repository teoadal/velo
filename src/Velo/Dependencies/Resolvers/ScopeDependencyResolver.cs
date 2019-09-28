namespace Velo.Dependencies.Resolvers
{
    internal sealed class ScopeDependencyResolver : DependencyResolver
    {
        private bool _addedToScope;

        public ScopeDependencyResolver(IDependency dependency, string dependencyName = null)
            : base(dependency, dependencyName)
        {
        }

        protected override void DestroyComplete()
        {
            _addedToScope = false;
        }

        protected override void ResolveComplete(object resolvedInstance, DependencyContainer container)
        {
            if (_addedToScope) return;
            
            DependencyScope.Register(this);
            _addedToScope = true;
        }
    }
}