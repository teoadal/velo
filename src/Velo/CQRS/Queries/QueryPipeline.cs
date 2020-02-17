using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryPipeline<TQuery, TResult> : IQueryPipeline<TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly QueryBehaviours<TQuery, TResult> _behaviours;
        private readonly IQueryPreProcessor<TQuery, TResult>[] _preProcessors;
        private readonly IQueryProcessor<TQuery, TResult> _processor;
        private readonly IQueryPostProcessor<TQuery, TResult>[] _postProcessors;

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

        public ValueTask<TResult> GetResponse(TQuery query, CancellationToken cancellationToken)
        {
            return _behaviours.HasBehaviours
                ? _behaviours.GetResponse(query, cancellationToken)
                : RunProcessors(query, cancellationToken);
        }

        internal async ValueTask<TResult> RunProcessors(TQuery query, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var preProcessor in _preProcessors)
            {
                var preProcess = preProcessor.PreProcess(query, cancellationToken);
                if (!preProcess.IsCompletedSuccessfully)
                {
                    await preProcess;
                }
            }

            var process = _processor.Process(query, cancellationToken);
            var result = process.IsCompletedSuccessfully
                ? process.Result
                : await process;

            foreach (var postProcessor in _postProcessors)
            {
                var postProcess = postProcessor.PostProcess(query, result, cancellationToken);
                if (!postProcess.IsCompletedSuccessfully)
                {
                    await postProcess;
                }
            }

            return result;
        }

        public ValueTask<TResult> GetResponse(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            return GetResponse((TQuery) query, cancellationToken);
        }
    }

    internal interface IQueryPipeline<TResult>
    {
        ValueTask<TResult> GetResponse(IQuery<TResult> query, CancellationToken cancellationToken);
    }
}