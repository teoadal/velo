using System;
using System.Reflection;

namespace Velo.DependencyInjection
{
    public interface IDependencyScope : IServiceProvider, IDisposable
    {
        event Action<IDependencyScope> Destroy;
        
        object Activate(Type implementation, ConstructorInfo constructor = null);
        
        object GetRequiredService(Type contract);

        T Activate<T>(ConstructorInfo constructor = null);
        
        T GetService<T>();
        
        T[] GetServices<T>();
        
        T GetRequiredService<T>();
    }
}