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

        private readonly Type _processorType;

        private readonly Type _parallelPipeline;
        private readonly Type _simplePipeline;
        private readonly Type _sequentialPipeline;

        public NotificationPipelineFactory()
        {
            _contract = typeof(INotificationPipeline<>);

            _processorType = typeof(INotificationProcessor<>);

            _parallelPipeline = typeof(NotificationParallelPipeline<>);
            _simplePipeline = typeof(NotificationSimplePipeline<>);
            _sequentialPipeline = typeof(NotificationSequentialPipeline<>);
        }

        public bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, _contract);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var contractGenericArgs = contract.GenericTypeArguments;
            var notificationType = contractGenericArgs[0];

            var processorType = _processorType.MakeGenericType(contractGenericArgs);
            var dependencies = engine.GetApplicable(processorType);
            var dependenciesLength = dependencies.Length;

            var pipelineType = dependenciesLength switch
            {
                0 => throw Error.DependencyNotRegistered(processorType),
                1 => _simplePipeline,
                _ => (ParallelAttribute.IsDefined(notificationType) ? _parallelPipeline : _sequentialPipeline)
            };

            var implementation = pipelineType.MakeGenericType(contractGenericArgs);
            var lifetime = dependencies.DefineLifetime();
            var resolver = DependencyResolver.Build(lifetime, implementation, engine);

            return Dependency.Build(lifetime, new[] {contract}, resolver);
        }
    }
}