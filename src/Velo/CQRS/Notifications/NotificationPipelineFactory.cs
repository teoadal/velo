using System;
using Velo.CQRS.Notifications.Pipeline;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Threading;
using Velo.Utils;

namespace Velo.CQRS.Notifications
{
    internal sealed class NotificationPipelineFactory : IDependencyFactory
    {
        public bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, Types.NotificationPipeline);
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