using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS.Commands;
using Velo.CQRS.Commands.Pipeline;
using Velo.Utils;

namespace Velo.Extensions.DependencyInjection.CQRS.Commands
{
    internal static class CommandPipelineFactory
    {
        private static readonly MethodInfo ActivatorMethod;

        static CommandPipelineFactory()
        {
            ActivatorMethod = typeof(CommandPipelineFactory)
                .GetMethod(nameof(Activator), BindingFlags.Static | BindingFlags.NonPublic)!;
        }
        
        public static Func<IServiceProvider, object> GetActivator(Type commandType)
        {
            var method = ActivatorMethod.MakeGenericMethod(commandType);
            return ReflectionUtils.BuildStaticMethodDelegate<Func<IServiceProvider, object>>(method);
        }
        
        private static object Activator<TCommand>(IServiceProvider provider)
            where TCommand : ICommand
        {
            var behaviours = provider.GetArray<ICommandBehaviour<TCommand>>();
            var preProcessors = provider.GetArray<ICommandPreProcessor<TCommand>>();
            var processor = provider.GetRequiredService<ICommandProcessor<TCommand>>();
            var postProcessors = provider.GetArray<ICommandPostProcessor<TCommand>>();

            if (behaviours.Length > 0)
            {
                return new CommandFullPipeline<TCommand>(behaviours, preProcessors, processor, postProcessors);
            }

            if (preProcessors.Length > 0 || postProcessors.Length > 0)
            {
                return new CommandSequentialPipeline<TCommand>(preProcessors, processor, postProcessors);
            }

            return new CommandSimplePipeline<TCommand>(processor);
        }
    }
}