using System.Reflection;
using Velo.CQRS;
using Velo.Extensions.DependencyInjection.CQRS;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class EmitterInstaller
    {
        public static IServiceCollection AddEmitter(this IServiceCollection services)
        {
            services.AddScoped<IEmitter>(provider => new Emitter(provider));
            return services;
        }

        public static IServiceCollection AddEmitterProcessorsFrom(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var scanner = new ProcessorsScanner(services, lifetime);
            scanner.Scan(assembly);
            return services;
        }
    }
}