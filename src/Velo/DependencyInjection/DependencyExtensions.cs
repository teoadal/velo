using Velo.Collections;
using Velo.DependencyInjection.Dependencies;

namespace Velo.DependencyInjection
{
    internal static class DependencyExtensions
    {
        public static DependencyLifetime DefineLifetime(this LocalList<IDependency> dependencies)
        {
            int singleton = 0, scoped = 0;
            foreach (var dependency in dependencies)
            {
                if (dependency == null) continue;
                
                switch (dependency.Lifetime)
                {
                    case DependencyLifetime.Singleton:
                        singleton++;
                        break;
                    case DependencyLifetime.Scoped:
                        scoped++;
                        break;
                }
            }

            if (singleton == dependencies.Length) return DependencyLifetime.Singleton;
            return scoped == dependencies.Length
                ? DependencyLifetime.Scoped
                : DependencyLifetime.Transient;
        }
    }
}