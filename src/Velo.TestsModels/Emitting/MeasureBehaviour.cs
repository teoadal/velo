using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;

namespace Velo.TestsModels.Emitting
{
    public class MeasureBehaviour : ICommandBehaviour<IMeasureCommand>
    {
        public TimeSpan Elapsed { get; private set; }

        public async Task Execute(IMeasureCommand command, Func<Task> next,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            await next();

            Elapsed = stopwatch.Elapsed;
        }
    }
}