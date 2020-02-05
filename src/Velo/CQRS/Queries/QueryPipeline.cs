using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryPipeline<TQuery, TResult> : IQueryPipeline<TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryPreProcessor<TQuery, TResult>[] _preProcessors;
        private readonly IQueryProcessor<TQuery, TResult> _processor;
        private readonly IQueryPostProcessor<TQuery, TResult>[] _postProcessors;

        public QueryPipeline(
            IQueryPreProcessor<TQuery, TResult>[] preProcessors,
            IQueryProcessor<TQuery, TResult> processor,
            IQueryPostProcessor<TQuery, TResult>[] postProcessors)
        {
            _preProcessors = preProcessors;
            _processor = processor;
            _postProcessors = postProcessors;
        }

        public QueryPipeline(IQueryProcessor<TQuery, TResult> processor)
            : this(Array.Empty<IQueryPreProcessor<TQuery, TResult>>(), processor, Array.Empty<IQueryPostProcessor<TQuery, TResult>>())
        {
            
        }
        
        public async ValueTask<TResult> GetResponse(TQuery query, CancellationToken cancellationToken)
        {
            foreach (var preProcessor in _preProcessors)
            {
                var preProcess = preProcessor.PreProcess(query, cancellationToken);
                if (!preProcess.IsCompletedSuccessfully) await preProcess;
            }

            var processorTask = _processor.Process(query, cancellationToken);
            var result = processorTask.IsCompletedSuccessfully
                ? processorTask.Result
                : await processorTask;

            foreach (var postProcessor in _postProcessors)
            {
                if (cancellationToken.IsCancellationRequested) return result;
                var postProcess = postProcessor.PostProcess(query, result, cancellationToken);
                result = postProcess.IsCompletedSuccessfully
                    ? postProcess.Result
                    : await postProcess;
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