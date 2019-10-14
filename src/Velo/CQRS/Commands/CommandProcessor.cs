using System.Threading;
using System.Threading.Tasks;
using Velo.DependencyInjection;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandProcessor<TNotification> : ICommandProcessor
    {
        public async Task Execute(DependencyProvider scope, TNotification notification,
            CancellationToken cancellationToken)
        {
            var handlers = scope.GetService<ICommandHandler<TNotification>[]>();
            
            for (var i = 0; i < handlers.Length; i++)
            {
                await handlers[i].Handle(notification, cancellationToken);
            }
        }
    }

    internal interface ICommandProcessor
    {
        
    }
}