using System;

namespace Velo.Dependencies.Resolvers
{
    public interface IDependencyResolver
    {
        bool Applicable(Type contract, string parameterName = null);
        
        void Destroy();
        
        object Resolve(Type contract, DependencyContainer container);
    }
}