using System;
using Velo.Collections;
using Velo.DependencyInjection.Dependencies;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    internal static class DependencyExtensions
    {
        public static DependencyLifetime DefineLifetime(this LocalList<DependencyLifetime> lifetimes)
        {
            int singleton = 0, transient = 0;
            foreach (var lifetime in lifetimes)
            {
                switch (lifetime)
                {
                    case DependencyLifetime.Singleton:
                        singleton++;
                        break;
                    case DependencyLifetime.Transient:
                        transient++;
                        break;
                }
            }

            if (singleton == lifetimes.Length) return DependencyLifetime.Singleton;
            return transient == 0
                ? DependencyLifetime.Scoped
                : DependencyLifetime.Transient;
        }

        public static DependencyLifetime DefineLifetime(this IDependency[] dependencies)
        {
            var lifetimes = new LocalList<DependencyLifetime>();

            foreach (var dependency in dependencies)
            {
                if (dependency == null) continue;
                lifetimes.Add(dependency.Lifetime);
            }

            return DefineLifetime(lifetimes);
        }

        public static DependencyLifetime DefineLifetime(this IDependencyEngine engine, Type implementation)
        {
            var constructor = ReflectionUtils.GetConstructor(implementation);
            var parameters = constructor.GetParameters();

            var dependencies = new LocalList<DependencyLifetime>();
            foreach (var parameter in parameters)
            {
                var required = !parameter.HasDefaultValue;
                var dependency = engine.GetDependency(parameter.ParameterType, required);

                if (dependency != null)
                {
                    dependencies.Add(dependency.Lifetime);
                }
            }

            return dependencies.DefineLifetime();
        }
    }
}