using Velo.CQRS.Queries;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

namespace Velo.CQRS
{
    public static class EmitterExtensions
    {
        public static DependencyCollection AddMediator(this DependencyCollection collection)
        {
            collection.Add(Typeof<Emitter>.Raw, new EmitterResolver());

            return collection;
        }

        public static DependencyCollection AddRequestHandler<THandler>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where THandler : IQueryHandler
        {
            var implementation = Typeof<THandler>.Raw;
            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(implementation, QueryRouter.HandlerType);

            collection.Add(contracts.ToArray(), implementation, lifetime);

            return collection;
        }

        public static DependencyScanner AddMediatorHandlers(this DependencyScanner scanner,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            scanner.UseAllover(new EmitterAllover(lifetime));

            return scanner;
        }
    }
}