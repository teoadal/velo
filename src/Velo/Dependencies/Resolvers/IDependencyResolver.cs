using System;

namespace Velo.Dependencies.Resolvers
{
    public interface IDependencyResolver
    {
        bool Applicable(Type contract, string parameterName = null);

        void Init(DependencyContainer container);
        
        void Destroy();
        
        object Resolve(Type contract, DependencyContainer container);
    }
}