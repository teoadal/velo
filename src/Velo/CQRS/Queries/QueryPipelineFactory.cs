using System;
using Velo.CQRS.Queries.Pipeline;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Velo.Utils;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryPipelineFactory : IDependencyFactory
    {
        public bool Applicable(Type contract)
        {
            return ReflectionUtils.IsGenericTypeImplementation(contract, Types.QueryPipeline);
        }

        public IDependency BuildDependency(Type contract, IDependencyEngine engine)
        {
            var genericArgs = contract.GenericTypeArguments;

            Type pipelineType;
            if (ExistsBehaviour(genericArgs, engine))
            {
                pipelineType = typeof(QueryFullPipeline<,>);
            }
            else if (ExistsPreOrPostProcessor(genericArgs, engine))
            {
                pipelineType = typeof(QuerySequentialPipeline<,>);
            }
            else
            {
                pipelineType = typeof(QuerySimplePipeline<,>);
            }

            var implementation = pipelineType.MakeGenericType(genericArgs);
            var lifetime = engine.DefineLifetime(implementation);
            var resolver = DependencyResolver.Build(lifetime, implementation, engine);

            return Dependency.Build(lifetime, new[] {contract}, resolver);
        }

        private bool ExistsBehaviour(Type[] contractGenericArgs, IDependencyEngine engine)
        {
            var behaviourType = typeof(IQueryBehaviour<,>).MakeGenericType(contractGenericArgs);
            return engine.Contains(behaviourType);
        }

        private bool ExistsPreOrPostProcessor(Type[] contractGenericArgs, IDependencyEngine engine)
        {
            var preProcessorType = typeof(IQueryPreProcessor<,>).MakeGenericType(contractGenericArgs);
            if (engine.Contains(preProcessorType)) return true;

            var postProcessorType = typeof(IQueryPostProcessor<,>).MakeGenericType(contractGenericArgs);
            return engine.Contains(postProcessorType);
        }
    }
}