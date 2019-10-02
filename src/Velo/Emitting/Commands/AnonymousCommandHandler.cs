using System;
using Velo.Dependencies;

namespace Velo.Emitting.Commands
{
    internal sealed class AnonymousCommandHandler<TCommand> : EmitterContext, ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly Action<EmitterContext, TCommand> _handler;

        internal AnonymousCommandHandler(DependencyContainer container, Action<EmitterContext, TCommand> handler) :
            base(container)
        {
            _handler = handler;
        }

        public void Execute(HandlerContext<TCommand> context)
        {
            _handler(this, context.Payload);
        }
    }
}