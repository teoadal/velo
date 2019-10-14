using Velo.DependencyInjection.Resolvers;

namespace Velo.DependencyInjection.Dependencies
{
    internal sealed class TransientDependency : Dependency
    {
        public TransientDependency(DependencyResolver resolver) : base(resolver)
        {
        }

        public override object GetInstance(DependencyProvider scope)
        {
            return Resolver.Resolve(scope);
        }
    }
}