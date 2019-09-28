using System;

namespace Velo.Dependencies
{
    public interface IDependency
    {
        bool Applicable(Type contract);

        void Destroy();

        void Init(DependencyContainer container);
        
        object Resolve(Type contract, DependencyContainer container);
    }
}