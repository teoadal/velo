using System;
using Velo.CQRS.Commands.Pipeline;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.CQRS.Commands
{
    public class CommandPipelineFactory : IDependencyFactory
    {
        public bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, Types.CommandPipeline);
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
            var resolver = DependencyResolver.Build(lifetime, implementation);

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