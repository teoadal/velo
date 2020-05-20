using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.Threading;
using Velo.Utils;

namespace Velo.CQRS.Commands
{
    internal sealed class ActionCommandProcessor<TCommand> : ICommandProcessor<TCommand>, IDisposable
        where TCommand : ICommand
    {
        private Action<TCommand> _processor;

        private bool _disposed;
        
        public ActionCommandProcessor(Action<TCommand> processor)
        {
            _processor = processor;
        }

        public Task Process(TCommand command, CancellationToken cancellationToken)
        {
            if (_disposed) throw Error.Disposed(nameof(ICommandProcessor<TCommand>));
            
            _processor(command);
            return TaskUtils.CompletedTask;
        }

        public void Dispose()
        {
            _processor = null!;
            _disposed = true;
        }
    }
}