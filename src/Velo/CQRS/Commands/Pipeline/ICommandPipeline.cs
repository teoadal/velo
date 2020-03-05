using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Commands.Pipeline
{
    internal interface ICommandPipeline<in TCommand> : ICommandPipeline
    {
        Task Execute(TCommand command, CancellationToken cancellationToken);
    }

    internal interface ICommandPipeline : IDisposable
    {
        Task Send(ICommand command, CancellationToken cancellationToken);
    }
}