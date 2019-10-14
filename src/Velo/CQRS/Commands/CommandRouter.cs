using System;
using System.Collections.Concurrent;
using Velo.Utils;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandRouter
    {
        private static readonly Type ProcessorType = typeof(CommandProcessor<>);

        private readonly Func<Type, ICommandProcessor> _buildProcessor;
        private readonly ConcurrentDictionary<Type, ICommandProcessor> _processors;

        public CommandRouter()
        {
            _buildProcessor = BuildProcessor;
            _processors = new ConcurrentDictionary<Type, ICommandProcessor>();
        }

        public CommandProcessor<TNotification> GetProcessor<TNotification>()
        {
            var notificationType = Typeof<TNotification>.Raw;
            var processor = _processors.GetOrAdd(notificationType, _buildProcessor);
            return (CommandProcessor<TNotification>) processor;
        }

        private static ICommandProcessor BuildProcessor(Type notificationType)
        {
            var processorType = ProcessorType.MakeGenericType(notificationType);
            return (ICommandProcessor) Activator.CreateInstance(processorType);
        }
    }
}