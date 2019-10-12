using System.Threading;
using System.Threading.Tasks;
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

        public Task<TResult> AskAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            var processor = _queryProcessors.GetProcessor(query);
            return processor.ExecuteAsync(query, cancellationToken);
        }

        public Task<TResult> AskAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery: IQuery<TResult>
        {
            var processor = _queryProcessors.GetProcessor<TQuery, TResult>();
            return processor.ExecuteAsync(query, cancellationToken);
        }
        
        public Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            var processor = (CommandProcessor<TCommand>) _commandProcessors.GetProcessor<TCommand>();
            return processor.ExecuteAsync(command, cancellationToken);
        }

        public Task ProcessStoredAsync(CancellationToken cancellationToken = default)
        {
            var commandProcessors = _commandProcessors.Processors;
            
            var tasks = new Task[commandProcessors.Count];
            var index = 0;
            foreach (var commandProcessor in commandProcessors)
            {
                tasks[index++] = commandProcessor.ProcessStoredAsync(cancellationToken);
            }

            return Task.WhenAll(tasks);
        }
        
        public void Store<TCommand>(TCommand command) where TCommand: ICommand
        {
            var processor = (CommandProcessor<TCommand>) _commandProcessors.GetProcessor<TCommand>();
            processor.Store(command);
        }
    }
}