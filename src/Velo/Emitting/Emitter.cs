using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.Emitting.Commands;
using Velo.Emitting.Queries;

namespace Velo.Emitting
{
    public sealed class Emitter
    {
        private readonly CommandProcessorsCollection _commandProcessors;
        private readonly QueryProcessorsCollection _queryProcessors;

        public Emitter(IServiceProvider container)
        {
            _commandProcessors = new CommandProcessorsCollection(container);
            _queryProcessors = new QueryProcessorsCollection(container);
        }

        public TResult Ask<TResult>(IQuery<TResult> query)
        {
            var processor = (IQueryProcessor<TResult>) _queryProcessors.GetProcessor(query);
            return processor.Execute(query);
        }

        public Task<TResult> AskAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            var processor = (IAsyncQueryProcessor<TResult>) _queryProcessors.GetProcessor(query);
            return processor.ExecuteAsync(query, cancellationToken);
        }

        public void Execute<TCommand>(TCommand command) where TCommand : ICommand
        {
            var processor = (ICommandProcessor<TCommand>) _commandProcessors.GetProcessor<TCommand>();
            processor.Execute(command);
        }

        public Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            var processor = (IAsyncCommandProcessor<TCommand>) _commandProcessors.GetProcessor<TCommand>();
            return processor.ExecuteAsync(command, cancellationToken);
        }
    }
}