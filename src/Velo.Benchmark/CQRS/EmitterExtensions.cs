using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS.Queries;
using Velo.Utils;

namespace Velo.Benchmark.CQRS
{
    public static class EmitterExtensions
    {
        public static IServiceCollection AddQueryProcessor<TProcessor>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            var processorType = typeof(TProcessor);
            var contract = ReflectionUtils.GetGenericInterfaceImplementations(processorType, typeof(IQueryProcessor<,>))[0];

            services.AddSingleton(contract, processorType);

            var queryType = contract.GenericTypeArguments[0];
            var resultType = contract.GenericTypeArguments[1];
            var pipelineType = typeof(QueryPipeline<,>).MakeGenericType(queryType, resultType);

            var activator = BuildActivator(queryType, resultType);

            services.Add(new ServiceDescriptor(pipelineType, activator, lifetime));

            return services;
        }

        private static Func<IServiceProvider, object> BuildActivator(Type queryType, Type resultType)
        {
            var activatorMethod = typeof(EmitterExtensions).GetMethod(nameof(Activator), BindingFlags.Static | BindingFlags.NonPublic);
            var method = activatorMethod.MakeGenericMethod(queryType, resultType);
            return (Func<IServiceProvider, object>) method.Invoke(null, Array.Empty<object>());
        }

        private static Func<IServiceProvider, object> Activator<TQuery, TResult>()
            where TQuery : IQuery<TResult>
        {
            return ctx => new QueryPipeline<TQuery, TResult>(
                ctx.GetService<IEnumerable<IQueryBehaviour<TQuery, TResult>>>().ToArray(),
                ctx.GetService<IEnumerable<IQueryPreProcessor<TQuery, TResult>>>().ToArray(),
                ctx.GetRequiredService<IQueryProcessor<TQuery, TResult>>(),
                ctx.GetService<IEnumerable<IQueryPostProcessor<TQuery, TResult>>>().ToArray());
        }
    }
}