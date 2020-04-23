using System;
using System.Reflection;

namespace Velo.DependencyInjection
{
    public interface IDependencyScope : IServiceProvider, IDisposable
    {
        event Action<IDependencyScope>? Destroy;

        object Activate(Type implementation, ConstructorInfo? constructor = null);

        object GetRequiredService(Type contract);
    }
}