using Velo.CQRS;

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
    }
}