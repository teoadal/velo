namespace Velo.Dependencies.Resolvers
{
    internal sealed class DefaultResolver : Resolver
    {
        public DefaultResolver(IDependency dependency, string dependencyName = null)
            : base(dependency, dependencyName)
        {
        }
    }
}