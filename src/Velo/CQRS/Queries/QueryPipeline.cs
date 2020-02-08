using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    internal sealed class QueryPipeline<TQuery, TResult> : IQueryPipeline<TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryBehaviour<TQuery, TResult>[] _behaviours;
        private readonly IQueryPreProcessor<TQuery, TResult>[] _preProcessors;
        private readonly IQueryProcessor<TQuery, TResult> _processor;
        private readonly IQueryPostProcessor<TQuery, TResult>[] _postProcessors;

        public QueryPipeline(
            IQueryBehaviour<TQuery, TResult>[] behaviours,
            IQueryPreProcessor<TQuery, TResult>[] preProcessors,
            IQueryProcessor<TQuery, TResult> processor,
            IQueryPostProcessor<TQuery, TResult>[] postProcessors)
        {
            _behaviours = behaviours;
            _preProcessors = preProcessors;
            _processor = processor;
            _postProcessors = postProcessors;
        }

        public ValueTask<TResult> GetResponse(TQuery query, CancellationToken cancellationToken)
        {
            if (_behaviours.Length == 0) return RunProcessors(query, cancellationToken);
            
            var closure = new QueriesBehaviours<TQuery, TResult>(query, _behaviours, this, cancellationToken);
            return closure.GetResponse();
        }

        internal async ValueTask<TResult> RunProcessors(TQuery query, CancellationToken cancellationToken)
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