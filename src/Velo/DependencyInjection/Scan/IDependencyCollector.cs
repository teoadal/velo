using System;

namespace Velo.DependencyInjection.Scan
{
    public interface IDependencyCollector
    {
        void TryRegister(DependencyCollection collection, Type implementation);
    }
}