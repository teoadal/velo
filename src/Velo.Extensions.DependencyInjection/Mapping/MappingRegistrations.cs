using Microsoft.Extensions.DependencyInjection;
using Velo.Mapping;

namespace Velo.Extensions.DependencyInjection.Mapping
{
    public static class MappingRegistrations
    {
        public static IServiceCollection AddMapper(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IMapper<>), typeof(CompiledMapper<>));
            return services;
        }
    }
}