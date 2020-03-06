using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS.Queries;
using Velo.CQRS.Queries.Pipeline;
using Velo.Utils;

namespace Velo.Extensions.DependencyInjection.CQRS.Queries
{
    internal static class QueryPipelineFactory
    {
        private static readonly MethodInfo ActivatorMethod;

        static QueryPipelineFactory()
        {
            ActivatorMethod = typeof(QueryPipelineFactory)
                .GetMethod(nameof(Activator), BindingFlags.Static | BindingFlags.NonPublic);
        }

        public static Func<IServiceProvider, object> GetActivator(Type queryType, Type resultType)
        {
            var method = ActivatorMethod.MakeGenericMethod(queryType, resultType);
            return ReflectionUtils.BuildStaticMethodDelegate<Func<IServiceProvider, object>>(method);
        }

        private static object Activator<TQuery, TResult>(IServiceProvider provider)
            where TQuery : IQuery<TResult>
        {
            var behaviours = provider.GetArray<IQueryBehaviour<TQuery, TResult>>();
            var preProcessors = provider.GetArray<IQueryPreProcessor<TQuery, TResult>>();
            var processor = provider.GetRequiredService<IQueryProcessor<TQuery, TResult>>();
            var postProcessors = provider.GetArray<IQueryPostProcessor<TQuery, TResult>>();

            if (behaviours.Length > 0)
            {
                return new QueryFullPipeline<TQuery, TResult>(behaviours, preProcessors, processor, postProcessors);
            }

            if (preProcessors.Length > 0 || postProcessors.Length > 0)
            {
                return new QuerySequentialPipeline<TQuery, TResult>(preProcessors, processor, postProcessors);
            }

            return new QuerySimplePipeline<TQuery, TResult>(processor);
        }
    }
}