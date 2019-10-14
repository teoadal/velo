using System;

namespace Velo.DependencyInjection.Scan
{
    public abstract class DependencyAllover
    {
        public abstract void TryRegister(DependencyCollection collection, Type implementation);
    }
}