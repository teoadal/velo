using System;
using System.Threading;
using System.Threading.Tasks;

namespace Velo.CQRS.Queries.Pipeline
{
    internal sealed partial class QueryFullPipeline<TQuery, TResult>
    {
        private sealed class BehaviourContext : IDisposable
        {
            [ThreadStatic] 
            private static Closure? _closure;

            private IQueryBehaviour<TQuery, TResult>[] _behaviours;
            private QueryFullPipeline<TQuery, TResult> _pipeline;

            public BehaviourContext(QueryFullPipeline<TQuery, TResult> pipeline,
                IQueryBehaviour<TQuery, TResult>[] behaviours)
            {
                _closure = null;
                _behaviours = behaviours;
                _pipeline = pipeline;
            }

            public Task<TResult> GetResponse(TQuery query, CancellationToken cancellationToken)
            {
                Closure closure = _closure ??= new Closure(this);

                _closure = null;

                closure.CancellationToken = cancellationToken;
                closure.Query = query;

                return closure.GetResponse();
            }

            private sealed class Closure : IDisposable
            {
                public CancellationToken CancellationToken;
                public TQuery Query;

                private BehaviourContext _context;
                private Func<Task<TResult>> _next;

                private int _position;

                public Closure(BehaviourContext context)
                {
                    _context = context;
                    Query = default!;
                    CancellationToken = default;

                    _next = GetResponse;
                    _position = 0;
                }

                public Task<TResult> GetResponse()
                {
                    var behaviours = _context._behaviours;

                    // ReSharper disable once InvertIf
                    if ((uint) _position < (uint) behaviours.Length)
                    {
                        var behaviour = behaviours[_position++];
                        return behaviour.Execute(Query, _next, CancellationToken);
                    }

                    var pipeline = _context._pipeline;
                    var query = Query;
                    var token = CancellationToken;

                    Clear();

                    return pipeline.RunProcessors(query, token);
                }

                private void Clear()
                {
                    CancellationToken = default;
                    Query = default!;

                    _position = 0;

                    _closure = this;
                }

                public void Dispose()
                {
                    _context = null!;
                    _next = null!;
                }
            }

            public void Dispose()
            {
                _behaviours = null!;

                _closure?.Dispose();
                _closure = null;

                _pipeline = null!;
            }
        }
    }
}