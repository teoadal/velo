using System;
using Velo.DependencyInjection.Dependencies;

namespace Velo.DependencyInjection.Factories
{
    public interface IDependencyFactory
    {
        bool Applicable(Type contract);

        IDependency BuildDependency(Type contract , IDependencyEngine engine);
    }
}