using System.Threading;
using System.Threading.Tasks;

namespace Velo.Emitting.Commands
{
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand: ICommand
    {
        public Task ExecuteAsync(TCommand command, CancellationToken cancellationToken)
        {
            Execute(command);
            return Task.CompletedTask;
        }

        protected abstract void Execute(TCommand command);
    }
}