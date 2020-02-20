using Velo.DependencyInjection;
using Velo.DependencyInjection.Factories;

namespace Velo.Mapping
{
    public static class MappingExtensions
    {
        public static DependencyCollection AddMapper(this DependencyCollection collection)
        {
            var contract = typeof(IMapper<>);
            var implementation = typeof(CompiledMapper<>);
            
            collection.AddFactory(new GenericFactory(contract, implementation, DependencyLifetime.Singleton));
            
            return collection;
        }
    }
}