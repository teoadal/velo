using System;
using Velo.DependencyInjection.Dependencies;

namespace Velo.DependencyInjection
{
    public interface IDependencyEngine : IDisposable
    {
        bool Contains(Type type);

        IDependency[] GetApplicable(Type contract);

        IDependency? GetDependency(Type contract);

        IDependency GetRequiredDependency(Type contract);
    }
}