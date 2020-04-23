using System;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Threading;

namespace Velo.ECS.Systems.Handler
{
    internal sealed class SystemHandlerFactory : IDependencyFactory
    {
        private readonly Type _contract;

        public SystemHandlerFactory()
        {
            _contract = typeof(ISystemHandler<>);
        }

        public bool Applicable(Type contract)
        {
            return contract.IsGenericType && contract.GetGenericTypeDefinition() == _contract;
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var systemType = contract.GenericTypeArguments[0];
            var dependencies = engine.GetApplicable(systemType);

            if (dependencies.Length == 0)
            {
                var nullHandlerType = typeof(SystemNullHandler<>).MakeGenericType(systemType);
                var nullHandler = Activator.CreateInstance(nullHandlerType);
                return new InstanceDependency(new[] {contract}, nullHandler);
            }

            var (hasParallel, hasSequential) = DefineTypes(dependencies);

            var handlerType = hasParallel
                ? hasSequential
                    ? typeof(SystemFullHandler<>)
                    : typeof(SystemParallelHandler<>)
                : typeof(SystemSequentialHandler<>);

            var implementation = handlerType.MakeGenericType(systemType);
            var resolver = DependencyResolver.Build(DependencyLifetime.Singleton, implementation, engine);
            return Dependency.Build(DependencyLifetime.Singleton, new[] {contract}, resolver);
        }

        private static (bool, bool) DefineTypes(IDependency[] dependencies)
        {
            bool hasParallels = false, hasSequential = false;

            foreach (var dependency in dependencies)
            {
                if (ParallelAttribute.IsDefined(dependency.Resolver.Implementation))
                {
                    hasParallels = true;
                }
                else
                {
                    hasSequential = true;
                }
            }

            return (hasParallels, hasSequential);
        }
    }
}