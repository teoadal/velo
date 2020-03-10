using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

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
                Closure closure;
                if (_closure == null) closure = new Closure(this, query, cancellationToken);
                else
                {
                    closure = _closure;

                    closure.Context = this;
                    closure.Query = query;
                    closure.CancellationToken = cancellationToken;
                }

                return closure.GetResponse();
            }

            private sealed class Closure
            {
                public CancellationToken CancellationToken;
                public BehaviourContext Context;
                public TQuery Query;

                private readonly Func<Task<TResult>> _next;
                private int _position;

                public Closure(BehaviourContext context, TQuery query,
                    CancellationToken cancellationToken)
                {
                    Context = context;
                    Query = query;
                    CancellationToken = cancellationToken;

                    _next = GetResponse;
                    _position = 0;
                }

                public Task<TResult> GetResponse()
                {
                    var behaviours = Context._behaviours;

                    // ReSharper disable once InvertIf
                    if ((uint) _position < (uint) behaviours.Length)
                    {
                        var behaviour = behaviours[_position++];
                        return behaviour.Execute(Query, _next, CancellationToken);
                    }

                    var pipelines = Context._pipeline;
                    var query = Query;
                    var token = CancellationToken;

                    Clear();

                    return pipelines.RunProcessors(query, token);
                }

                private void Clear()
                {
                    Context = null!;
                    CancellationToken = default;
                    Query = default!;

                    _position = 0;
                }
            }

            public void Dispose()
            {
                _behaviours = null!;
                _pipeline = null!;
            }
        }
    }
}

#nullable restore