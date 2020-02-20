using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.DependencyInjection;

namespace Velo.CQRS.Queries
{
    internal sealed class ActionQueryProcessor<TQuery, TResult> : IQueryProcessor<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly Func<TQuery, TResult> _processor;

        public ActionQueryProcessor(Func<TQuery, TResult> processor)
        {
            _processor = processor;
        }

        public Task<TResult> Process(TQuery query, CancellationToken cancellationToken)
        {
            var result = _processor(query);
            return Task.FromResult(result);
        }
    }
    
    internal sealed class ActionQueryProcessor<TQuery, TContext, TResult> : IQueryProcessor<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly Func<TQuery, TContext, TResult> _processor;
        private readonly DependencyProvider _provider;

        public ActionQueryProcessor(Func<TQuery, TContext, TResult> processor, DependencyProvider provider)
        {
            _processor = processor;
            _provider = provider;
        }

        public Task<TResult> Process(TQuery query, CancellationToken cancellationToken)
        {
            var context = _provider.GetRequiredService<TContext>();
            var result = _processor(query, context);
            return Task.FromResult(result);
        }
    }
}