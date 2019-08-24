using System;

namespace Velo.Dependencies.Scan
{
    public interface IDependencyScanner
    {
        bool Applicable(Type type);

        void Register(DependencyBuilder builder, Type implementation);
    }
}