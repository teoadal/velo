using System;

namespace Velo.Dependencies
{
    public interface IDependency
    {
        bool Applicable(Type requestedType);

        object Resolve(Type requestedType, DependencyContainer container);
    }
}