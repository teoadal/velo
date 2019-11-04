using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.CQRS.Commands
{
    internal sealed class CommandProcessor<TNotification> : ICommandProcessor
    {
        private readonly Type _handlersArrayType;

        public CommandProcessor()
        {
            _handlersArrayType = Typeof<ICommandHandler<TNotification>[]>.Raw;
        }

        public async Task Execute(IServiceProvider scope, TNotification notification,
            CancellationToken cancellationToken)
        {
            var handlers = (ICommandHandler<TNotification>[]) scope.GetService(_handlersArrayType);
            
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