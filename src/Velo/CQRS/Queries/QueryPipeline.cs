using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryPipeline<TQuery, TResult> : IQueryPipeline<TResult>
        where TQuery : IQuery<TResult>
    {
        private QueryBehaviours<TQuery, TResult> _behaviours;
        private IQueryPreProcessor<TQuery, TResult>[] _preProcessors;
        private IQueryProcessor<TQuery, TResult> _processor;
        private IQueryPostProcessor<TQuery, TResult>[] _postProcessors;

        public QueryPipeline(
            IQueryBehaviour<TQuery, TResult>[] behaviours,
            IQueryPreProcessor<TQuery, TResult>[] preProcessors,
            IQueryProcessor<TQuery, TResult> processor,
            IQueryPostProcessor<TQuery, TResult>[] postProcessors)
        {
            _behaviours = new QueryBehaviours<TQuery, TResult>(this, behaviours);
            _preProcessors = preProcessors;
            _processor = processor;
            _postProcessors = postProcessors;
        }

        public Task<TResult> GetResponse(TQuery query, CancellationToken cancellationToken)
        {
            return _behaviours.HasBehaviours
                ? _behaviours.GetResponse(query, cancellationToken)
                : RunProcessors(query, cancellationToken);
        }

        internal async Task<TResult> RunProcessors(TQuery query, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var preProcessor in _preProcessors)
            {
                await preProcessor.PreProcess(query, cancellationToken);
            }

            var result = await _processor.Process(query, cancellationToken);

            foreach (var postProcessor in _postProcessors)
            {
                await postProcessor.PostProcess(query, result, cancellationToken);
            }

            return result;
        }

        public Task<TResult> GetResponse(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            return GetResponse((TQuery) query, cancellationToken);
        }

        public void Dispose()
        {
            _behaviours.Dispose();

            _behaviours = null;
            _preProcessors = null;
            _processor = null;
            _postProcessors = null;
        }
    }

    internal interface IQueryPipeline<TResult> : IDisposable
    {
        Task<TResult> GetResponse(IQuery<TResult> query, CancellationToken cancellationToken);
    }
}