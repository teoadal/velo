using System;
using System.Threading;
using System.Threading.Tasks;
using Velo.Utils;

namespace Velo.CQRS.Queries
{
    internal interface IQueryBehaviours<in TQuery, TResult> : IDisposable
        where TQuery : IQuery<TResult>
    {
        bool HasBehaviours { get; }

        Task<TResult> GetResponse(TQuery query, CancellationToken cancellationToken);
    }

    internal sealed class QueryBehaviours<TQuery, TResult> : IQueryBehaviours<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public bool HasBehaviours => true;

        [ThreadStatic] private static Closure _closure;

        private IQueryBehaviour<TQuery, TResult>[] _behaviours;
        private QueryPipeline<TQuery, TResult> _pipeline;

        public QueryBehaviours(QueryPipeline<TQuery, TResult> pipeline, IQueryBehaviour<TQuery, TResult>[] behaviours)
        {
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
            public QueryBehaviours<TQuery, TResult> Context;
            public TQuery Query;

            private readonly Func<Task<TResult>> _next;
            private int _position;

            public Closure(QueryBehaviours<TQuery, TResult> context, TQuery query, CancellationToken cancellationToken)
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
                Context = null;
                CancellationToken = default;
                Query = default;

                _position = 0;
                _closure = this;
            }
        }

        public void Dispose()
        {
            _behaviours = null;
            _pipeline = null;
        }
    }

    internal sealed class NullQueryBehaviours<TQuery, TResult> : IQueryBehaviours<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public static readonly IQueryBehaviours<TQuery, TResult> Instance = new NullQueryBehaviours<TQuery, TResult>();

        public bool HasBehaviours => false;

        private NullQueryBehaviours()
        {
        }

        public Task<TResult> GetResponse(TQuery query, CancellationToken cancellationToken)
        {
            throw Error.InvalidOperation("No behaviours for execute");
        }

        public void Dispose()
        {
        }
    }
}