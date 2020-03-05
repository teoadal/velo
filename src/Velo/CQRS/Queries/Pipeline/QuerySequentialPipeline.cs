using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries.Pipeline
{
    public class QuerySequentialPipeline<TQuery, TResult> : IQueryPipeline<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private IQueryPreProcessor<TQuery, TResult>[] _preProcessors;
        private IQueryProcessor<TQuery, TResult> _processor;
        private IQueryPostProcessor<TQuery, TResult>[] _postProcessors;

        public QuerySequentialPipeline(
            IQueryPreProcessor<TQuery, TResult>[] preProcessors, 
            IQueryProcessor<TQuery, TResult> processor, 
            IQueryPostProcessor<TQuery, TResult>[] postProcessors)
        {
            _preProcessors = preProcessors;
            _processor = processor;
            _postProcessors = postProcessors;
        }
        
        public async Task<TResult> GetResponse(TQuery query, CancellationToken cancellationToken)
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

        Task<TResult> IQueryPipeline<TResult>.GetResponse(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            return GetResponse((TQuery) query, cancellationToken);
        }

        public void Dispose()
        {
            _preProcessors = null;
            _processor = null;
            _postProcessors = null;
        }
    }
}