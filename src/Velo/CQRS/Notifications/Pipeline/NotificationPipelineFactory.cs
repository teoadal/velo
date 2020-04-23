using System;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Threading;
using Velo.Utils;

namespace Velo.CQRS.Notifications.Pipeline
{
    internal sealed class NotificationPipelineFactory : IDependencyFactory
    {
        private readonly Type _contract;

        public NotificationPipelineFactory()
        {
            _contract = typeof(INotificationPipeline<>);
        }

        public bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, _contract);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var contractGenericArgs = contract.GenericTypeArguments;
            var notificationType = contractGenericArgs[0];

            var processorType = typeof(INotificationProcessor<>).MakeGenericType(contractGenericArgs);
            var dependencies = engine.GetApplicable(processorType);
            var dependenciesLength = dependencies.Length;

            var pipelineType = dependenciesLength switch
            {
                0 => throw Error.DependencyNotRegistered(processorType),
                1 => typeof(NotificationSimplePipeline<>),
                _ => ParallelAttribute.IsDefined(notificationType) 
                    ? typeof(NotificationParallelPipeline<>) 
                    : typeof(NotificationSequentialPipeline<>)
            };

            var implementation = pipelineType.MakeGenericType(contractGenericArgs);
            var lifetime = dependencies.DefineLifetime();
            var resolver = DependencyResolver.Build(lifetime, implementation, engine);

            return Dependency.Build(lifetime, new[] {contract}, resolver);
        }
    }
}