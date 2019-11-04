using Velo.CQRS.Commands;
using Velo.CQRS.Queries;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Scan;
using Velo.Utils;

namespace Velo.CQRS
{
    public static class EmitterExtensions
    {
        public static DependencyCollection AddEmitter(this DependencyCollection collection)
        {
            collection.AddSingleton<CommandRouter>();
            collection.AddSingleton<QueryRouter>();
            collection.AddScoped<Emitter>();

            return collection;
        }

        public static DependencyCollection AddCommandHandler<THandler>(this DependencyCollection collection,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
            where THandler : ICommandHandler
        {
            var implementation = Typeof<THandler>.Raw;
            var contracts = ReflectionUtils.GetGenericInterfaceImplementations(implementation, CommandRouter.HandlerType);

            collection.Add(contracts.ToArray(), implementation, lifetime);

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

        public static DependencyScanner AddEmitterHandlers(this DependencyScanner scanner,
            DependencyLifetime lifetime = DependencyLifetime.Singleton)
        {
            scanner.UseAllover(new EmitterAllover(lifetime));

            return scanner;
        }
    }
}