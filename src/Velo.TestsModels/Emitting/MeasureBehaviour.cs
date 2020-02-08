using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;

namespace Velo.TestsModels.Emitting
{
    public class MeasureBehaviour<TCommand>: ICommandBehaviour<TCommand>
        where TCommand: ICommand
    {
        public TimeSpan Elapsed { get; private set; }
        
        public async ValueTask Execute(TCommand command, Func<ValueTask> next, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            
            await next();
            
            Elapsed = stopwatch.Elapsed;
        }
    }
}