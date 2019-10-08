using System;
using System.Collections.Concurrent;
using Velo.Utils;

namespace Velo.Emitting.Commands.Processors
{
    internal abstract class CommandProcessor<TCommand>: ICommandProcessor<TCommand>
        where TCommand: ICommand
    {
        private readonly Type _commandType;
        private readonly ConcurrentQueue<TCommand> _stored;

        protected CommandProcessor()
        {
            _commandType = Typeof<TCommand>.Raw;
            _stored = new ConcurrentQueue<TCommand>();
        }

        public bool Applicable(Type type)
        {
            return _commandType.IsAssignableFrom(type);
        }

        public void ProcessStored()
        {
            while (_stored.TryDequeue(out var command))
            {
                Execute(command);
            }
        }

        public abstract void Execute(TCommand command);

        public void Store(TCommand command)
        {
            _stored.Enqueue(command);
        }
    }
}