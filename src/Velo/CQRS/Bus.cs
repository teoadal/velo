using Velo.CQRS.Commands;
using Velo.CQRS.Queries;
using Velo.Dependencies;

namespace Velo.CQRS
{
    public sealed class Bus
    {
        private readonly CommandProcessorsCollection _commandProcessors;
        private readonly QueryProcessorsCollection _queryProcessors;

        public Bus(DependencyContainer container)
        {
            _commandProcessors = new CommandProcessorsCollection(container);
            _queryProcessors = new QueryProcessorsCollection(container);
        }

        public TResult Ask<TResult>(IQuery<TResult> query)
        {
            var processor = _queryProcessors.GetProcessor(query);
            return processor.Execute(query);
        }

        public TResult Ask<TQuery, TResult>(TQuery query) where TQuery: IQuery<TResult>
        {
            var processor = _queryProcessors.GetHandler<TQuery, TResult>();
            return processor.Execute(query);
        }
        
        public void Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            var processor = _commandProcessors.GetProcessor<TCommand>();
            processor.Execute(command);
        }
    }
}