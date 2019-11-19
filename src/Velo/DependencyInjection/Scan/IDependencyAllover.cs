using System;

namespace Velo.DependencyInjection.Scan
{
    public interface IDependencyAllover
    {
        void TryRegister(DependencyCollection collection, Type implementation);
    }
}