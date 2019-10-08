using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Emitting.Commands.Processors
{
    internal sealed class CommandProcessorsCollection
    {
        public Dictionary<int, ICommandProcessor>.ValueCollection Processors => _processors.Values;
        
        private readonly ConcurrentDictionary<Type, ICommandProcessor> _applicableProcessors;
        private readonly Dictionary<int, ICommandProcessor> _processors;
        private readonly Func<Type, ICommandProcessor> _findProcessor;

        public CommandProcessorsCollection(IServiceProvider container)
        {
            _applicableProcessors = new ConcurrentDictionary<Type, ICommandProcessor>();
            _findProcessor = FindProcessor;
            
            _processors = new CommandProcessorBuilder(container).CollectProcessors();
        }

        public ICommandProcessor GetProcessor<TCommand>() where TCommand : ICommand
        {
            var commandId = Typeof<TCommand>.Id;

            if (_processors.TryGetValue(commandId, out var existsHandler))
            {
                return existsHandler;
            }

            var commandType = Typeof<TCommand>.Raw;
            var applicableProcessor = _applicableProcessors.GetOrAdd(commandType, _findProcessor);

            if (applicableProcessor == null)
            {
                throw Error.NotFound($"Command handler for command '{typeof(TCommand).Name}' is not registered");
            }

            return applicableProcessor;
        }

        private ICommandProcessor FindProcessor(Type type)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var processor in _processors.Values)
            {
                if (processor.Applicable(type)) return processor;
            }

            return null;
        }
    }
}