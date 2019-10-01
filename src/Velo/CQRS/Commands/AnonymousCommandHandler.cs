using System;
using Velo.Dependencies;

namespace Velo.CQRS.Commands
{
    internal sealed class AnonymousCommandHandler<TCommand> : HandlerContext, ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly Action<HandlerContext, TCommand> _handler;

        internal AnonymousCommandHandler(DependencyContainer container, Action<HandlerContext, TCommand> handler) :
            base(container)
        {
            _handler = handler;
        }

        public void Execute(TCommand payload)
        {
            _handler(this, payload);
        }
    }
}