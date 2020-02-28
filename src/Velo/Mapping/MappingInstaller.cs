using Velo.DependencyInjection.Factories;
using Velo.Mapping;

// ReSharper disable once CheckNamespace
namespace Velo.DependencyInjection
{
    public static class MappingInstaller
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