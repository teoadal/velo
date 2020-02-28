using Velo.Mapping;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class MappingInstaller
    {
        public static IServiceCollection AddMapper(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IMapper<>), typeof(CompiledMapper<>));
            return services;
        }
    }
}