using Velo.CQRS.Commands;
using Velo.CQRS.Queries;
using Velo.Dependencies;

namespace Velo.CQRS
{
    public sealed class Bus
    {
        private readonly CommandProcessorsCollection _commandProcessorses;
        private readonly QueryHandlerCollection _queryHandlers;

        public Bus(DependencyContainer container)
        {
            _commandProcessorses = new CommandProcessorsCollection(container);
            _queryHandlers = new QueryHandlerCollection(container);
        }

        public TResult Ask<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
        {
            var queryHandler = _queryHandlers.GetHandler<TQuery, TResult>();
            return queryHandler.Execute(query);
        }

        public void Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            var commandHandler = _commandProcessorses.GetProcessor<TCommand>();
            commandHandler.Execute(command);
        }
    }
}