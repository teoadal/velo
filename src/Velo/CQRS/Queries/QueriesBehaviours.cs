using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries
{
    internal sealed class QueriesBehaviours<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly IQueryBehaviour<TQuery, TResult>[] _behaviours;
        private readonly CancellationToken _cancellationToken;
        private readonly TQuery _query;
        private readonly Func<ValueTask<TResult>> _next;
        private readonly QueryPipeline<TQuery, TResult> _pipeline;

        private int _position;

        public QueriesBehaviours(
            TQuery query,
            IQueryBehaviour<TQuery, TResult>[] behaviours,
            QueryPipeline<TQuery, TResult> pipeline,
            CancellationToken cancellationToken)
        {
            _query = query;
            _behaviours = behaviours;
            _pipeline = pipeline;
            _cancellationToken = cancellationToken;

            // ReSharper disable once ConvertClosureToMethodGroup
            _next = () => GetResponse();

            _position = 0;
        }

        public ValueTask<TResult> GetResponse()
        {
            // ReSharper disable once InvertIf
            if ((uint) _position < (uint) _behaviours.Length)
            {
                var behaviour = _behaviours[_position++];
                return behaviour.Execute(_query, _next, _cancellationToken);
            }

            return _pipeline.RunProcessors(_query, _cancellationToken);
        }
    }
}