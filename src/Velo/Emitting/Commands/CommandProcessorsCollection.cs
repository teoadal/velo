using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Velo.Dependencies;
using Velo.Utils;

namespace Velo.Emitting.Commands
{
    internal sealed class CommandProcessorsCollection
    {
        public ICollection<ICommandProcessor> Processors => _processors.Values;

        private readonly DependencyContainer _container;
        private readonly Func<Type, ICommandProcessor> _findProcessor;
        private readonly ConcurrentDictionary<Type, ICommandProcessor> _processors;

        public CommandProcessorsCollection(DependencyContainer container)
        {
            _container = container;
            _findProcessor = FindProcessor;
            _processors = new ConcurrentDictionary<Type, ICommandProcessor>();
        }

        public ICommandProcessor GetProcessor<TCommand>() where TCommand : ICommand
        {
            var commandType = Typeof<TCommand>.Raw;
            return _processors.GetOrAdd(commandType, _findProcessor);
        }

        private ICommandProcessor FindProcessor(Type commandType)
        {
            var processorType = typeof(CommandProcessor<>).MakeGenericType(commandType);
            return (ICommandProcessor) Activator.CreateInstance(processorType, _container);
        }
    }
}