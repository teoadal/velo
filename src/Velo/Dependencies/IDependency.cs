using System;

namespace Velo.Dependencies
{
    public interface IDependency
    {
        bool Applicable(Type contract);

        void Destroy();
        
        object Resolve(Type contract, DependencyContainer container);
    }
}