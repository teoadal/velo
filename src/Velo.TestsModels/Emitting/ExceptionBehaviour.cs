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
        
        public async ValueTask Execute(TCommand command, Func<ValueTask> next, CancellationToken cancellationToken)
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