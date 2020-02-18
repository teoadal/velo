using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;

namespace Velo.TestsModels.Emitting
{
    public class ExceptionBehaviour<TCommand>: ICommandBehaviour<TCommand>
        where TCommand: ICommand
    {
        public Exception Exception { get; private set; }
        
        public async Task Execute(TCommand command, Func<Task> next, CancellationToken cancellationToken)
        {
            try
            {
                await next();
            }
            catch (Exception e)
            {
                Exception = e;
            }
        }
    }
}