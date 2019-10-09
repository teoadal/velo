using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.Dependencies;

namespace Velo.Emitting.Commands
{
    internal sealed class AnonymousCommandHandler<TCommand> : EmitterContext, ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly Func<EmitterContext, TCommand, Task> _asyncHandler;
        private readonly Action<EmitterContext, TCommand> _syncHandler;

        internal AnonymousCommandHandler(DependencyContainer container,
            Func<EmitterContext, TCommand, Task> asyncHandler) :
            base(container)
        {
            _asyncHandler = asyncHandler;
        }

        internal AnonymousCommandHandler(DependencyContainer container, Action<EmitterContext, TCommand> syncHandler) :
            base(container)
        {
            _syncHandler = syncHandler;
        }

        public Task ExecuteAsync(TCommand command, CancellationToken cancellationToken)
        {
            if (_asyncHandler != null) return _asyncHandler(this, command);

            _syncHandler(this, command);
            return Task.CompletedTask;
        }
    }
}