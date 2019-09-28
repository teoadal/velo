namespace Velo.Dependencies.Resolvers
{
    internal sealed class DefaultDependencyResolver : DependencyResolver
    {
        public DefaultDependencyResolver(IDependency dependency, string dependencyName = null)
            : base(dependency, dependencyName)
        {
        }
    }
}