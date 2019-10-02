using Velo.Dependencies;
using Velo.Emitting.Commands;
using Velo.Emitting.Queries;

namespace Velo.Emitting
{
    public sealed class Emitter
    {
        private readonly CommandProcessorsCollection _commandProcessors;
        private readonly QueryProcessorsCollection _queryProcessors;

        public Emitter(DependencyContainer container)
        {
            _commandProcessors = new CommandProcessorsCollection(container);
            _queryProcessors = new QueryProcessorsCollection(container);
        }

        public TResult Ask<TResult>(IQuery<TResult> query)
        {
            var processor = _queryProcessors.GetProcessor(query);
            return processor.Execute(query);
        }

        public TResult Ask<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
        {
            var processor = _queryProcessors.GetHandler<TQuery, TResult>();
            
            var context = new HandlerContext<TQuery>(query);
            return processor.Execute(context);
        }

        public void Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            var processor = _commandProcessors.GetProcessor<TCommand>();
            processor.Execute(command);
        }
    }
}