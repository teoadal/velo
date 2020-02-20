using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.DependencyInjection;
using Velo.Utils;

namespace Velo.CQRS.Commands
{
    internal sealed class ActionCommandProcessor<TCommand> : ICommandProcessor<TCommand>
        where TCommand: ICommand
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
        where TCommand: ICommand
    {
        private readonly Action<TCommand, TContext> _processor;
        private readonly DependencyProvider _provider;

        public ActionCommandProcessor(Action<TCommand, TContext> processor, DependencyProvider provider)
        {
            _processor = processor;
            _provider = provider;
        }

        public Task Process(TCommand command, CancellationToken cancellationToken)
        {
            var context = _provider.GetRequiredService<TContext>();
            
            _processor(command, context);
            
            return TaskUtils.CompletedTask;
        }
    }
}