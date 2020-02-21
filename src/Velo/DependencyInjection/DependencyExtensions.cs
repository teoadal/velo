using Velo.Collections;
using Velo.DependencyInjection.Dependencies;

namespace Velo.DependencyInjection
{
    internal static class DependencyExtensions
    {
        public static DependencyLifetime DefineLifetime(this LocalList<DependencyLifetime> lifetimes)
        {
            int singleton = 0, scoped = 0;
            foreach (var lifetime in lifetimes)
            {
                switch (lifetime)
                {
                    case DependencyLifetime.Singleton:
                        singleton++;
                        break;
                    case DependencyLifetime.Scoped:
                        scoped++;
                        break;
                }
            }

            if (singleton == lifetimes.Length) return DependencyLifetime.Singleton;
            return scoped == lifetimes.Length
                ? DependencyLifetime.Scoped
                : DependencyLifetime.Transient;
        }

        public static DependencyLifetime DefineLifetime(this LocalList<IDependency> dependencies)
        {
            var lifetimes = dependencies
                .Where(dependency => dependency != null)
                .Select(dependency => dependency.Lifetime);

            return DefineLifetime(lifetimes);
        }

        public static DependencyLifetime DefineLifetime(this DependencyLifetime[] lifetimes)
        {
            return DefineLifetime(new LocalList<DependencyLifetime>(lifetimes));
        }
    }
}