using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.DependencyInjection;
using Velo.Threading;

namespace Velo.CQRS.Commands
{
    internal sealed class ActionCommandProcessor<TCommand> : ICommandProcessor<TCommand>
        where TCommand : ICommand
    {
        private readonly Action<TCommand> _processor;

        public ActionCommandProcessor(Action<TCommand> processor)
        {
            _processor = processor;
        }

        public Task Process(TCommand command, CancellationToken cancellationToken)
        {
            _processor(command);
            return TaskUtils.CompletedTask;
        }
    }

    internal sealed class ActionCommandProcessor<TCommand, TContext> : ICommandProcessor<TCommand>
        where TCommand : ICommand
        where TContext : class
    {
        private readonly Action<TCommand, TContext> _processor;
        private readonly IDependencyScope _scope;

        public ActionCommandProcessor(Action<TCommand, TContext> processor, IDependencyScope scope)
        {
            _processor = processor;
            _scope = scope;
        }

        public Task Process(TCommand command, CancellationToken cancellationToken)
        {
            var context = _scope.GetRequiredService<TContext>();

            _processor(command, context);

            return TaskUtils.CompletedTask;
        }
    }
}