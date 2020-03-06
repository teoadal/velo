using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.CQRS.Commands;

namespace Velo.TestsModels.Emitting.Boos.Create
{
    public class Behaviour : ICommandBehaviour<Command>
    {
        public Task Execute(Command command, Func<Task> next, CancellationToken cancellationToken)
        {
            return next();
        }
    }
}