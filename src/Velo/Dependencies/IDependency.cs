using System;

namespace Velo.Dependencies
{
    public interface IDependency
    {
        bool Applicable(Type requestedType);

        void Destroy();
        
        object Resolve(Type requestedType, DependencyContainer container);
    }
}