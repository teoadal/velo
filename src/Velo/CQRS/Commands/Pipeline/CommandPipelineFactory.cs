using System;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.CQRS.Commands.Pipeline
{
    public class CommandPipelineFactory : IDependencyFactory
    {
        private readonly Type _contract;

        public CommandPipelineFactory()
        {
            _contract = typeof(ICommandPipeline<>);
        }

        public bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, _contract);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var genericArgs = contract.GenericTypeArguments;

            Type pipelineType;
            if (ExistsBehaviour(genericArgs, engine))
            {
                pipelineType = typeof(CommandFullPipeline<>);
            }
            else if (ExistsPreOrPostProcessor(genericArgs, engine))
            {
                pipelineType = typeof(CommandSequentialPipeline<>);
            }
            else
            {
                pipelineType = typeof(CommandSimplePipeline<>);
            }

            var implementation = pipelineType.MakeGenericType(genericArgs);
            var lifetime = engine.DefineLifetime(implementation);
            var resolver = DependencyResolver.Build(lifetime, implementation, engine);

            return Dependency.Build(lifetime, new[] {contract}, resolver);
        }

        private bool ExistsBehaviour(Type[] contractGenericArgs, IDependencyEngine engine)
        {
            var behaviourType = typeof(ICommandBehaviour<>).MakeGenericType(contractGenericArgs);
            return engine.Contains(behaviourType);
        }

        private bool ExistsPreOrPostProcessor(Type[] contractGenericArgs, IDependencyEngine engine)
        {
            var preProcessorType = typeof(ICommandPreProcessor<>).MakeGenericType(contractGenericArgs);
            if (engine.Contains(preProcessorType)) return true;

            var postProcessorType = typeof(ICommandPostProcessor<>).MakeGenericType(contractGenericArgs);
            return engine.Contains(postProcessorType);
        }
    }
}