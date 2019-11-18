using System;
using System.Collections.Concurrent;
using Velo.Utils;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandRouter: IDisposable
    {
        public static readonly Type HandlerType = typeof(ICommandHandler<>);
        private static readonly Type ProcessorType = typeof(CommandProcessor<>);

        private Func<Type, ICommandProcessor> _buildProcessor;
        private ConcurrentDictionary<Type, ICommandProcessor> _processors;

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

        public void Dispose()
        {
            _buildProcessor = null!;
            
            CollectionUtils.DisposeValuesIfDisposable(_processors);
            _processors.Clear();
            _processors = null!;
        }
    }
}