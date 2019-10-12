using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Velo.Dependencies;

namespace Velo.Emitting.Commands
{
    internal sealed class CommandProcessor<TCommand> : ICommandProcessor
        where TCommand : ICommand
    {
        private readonly DependencyContainer _container;
        private readonly IDependency[] _handlerDependencies;
        private readonly Type _handlerType;
        private readonly ConcurrentQueue<TCommand> _stored;

        public CommandProcessor(DependencyContainer container, IDependency[] handlerDependencies)
        {
            _container = container;
            _handlerType = typeof(ICommandHandler<TCommand>);
            _handlerDependencies = handlerDependencies;

            _stored = new ConcurrentQueue<TCommand>();
        }

        public async Task ExecuteAsync(TCommand command, CancellationToken cancellationToken)
        {
            var dependencies = _handlerDependencies;
            for (var i = 0; i < dependencies.Length; i++)
            {
                var handler = (ICommandHandler<TCommand>) dependencies[i].Resolve(_handlerType, _container);
                await handler.ExecuteAsync(command, cancellationToken);

                if (command.StopPropagation) break;
            }
        }

        public async Task ProcessStoredAsync(CancellationToken cancellationToken)
        {
            var dependencies = _handlerDependencies;
            var handlers = new ICommandHandler<TCommand>[dependencies.Length];
            for (var i = 0; i < dependencies.Length; i++)
            {
                var handler = (ICommandHandler<TCommand>) dependencies[i].Resolve(_handlerType, _container);
                handlers[i] = handler;
            }

            while (_stored.TryDequeue(out var command))
            {
                for (var i = 0; i < handlers.Length; i++)
                {
                    await handlers[i].ExecuteAsync(command, cancellationToken);
                    if (command.StopPropagation) break;
                }
            }
        }

        public void Store(TCommand command)
        {
            _stored.Enqueue(command);
        }
    }
}