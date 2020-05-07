using System;
using System.Collections;
using System.Collections.Generic;
using Velo.Collections.Enumerators;
using Velo.Utils;

namespace Velo.ECS.Sources.Context
{
    internal sealed class EntitySourceContext<TEntity> : IEntitySourceContext<TEntity>
        where TEntity : class, IEntity
    {
        public bool IsStarted => _enumerator != null;

        private Enumerator? _enumerator;
        private readonly IServiceProvider _services;

        public EntitySourceContext(IServiceProvider services)
        {
            _services = services;
            _enumerator = null!;
        }

        public TEntity Get(int id)
        {
            if (_enumerator == null)
            {
                throw Error.InvalidOperation("Source context isn't started");
            }

            return _enumerator.TryGet(id, out var exists)
                ? exists
                : throw Error.NotFound($"Entity with id '{id}' isn't found in sources");
        }

        public IEnumerable<TEntity> GetEntities()
        {
            if (_enumerator != null) return _enumerator;

            // request here for avoid circular dependency exception
            var sources = (IEntitySource<TEntity>[]) _services.GetService(typeof(IEntitySource<TEntity>[]));

            if (sources == null || sources.Length == 0)
            {
                return EmptyEnumerator<TEntity>.Instance;
            }

            _enumerator = new Enumerator(this, sources);
            return _enumerator;
        }

        private sealed class Enumerator : IEnumerable<TEntity>, IEnumerator<TEntity>
        {
            // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
            public TEntity Current => _current;

            private Dictionary<int, TEntity> _buffer;
            private EntitySourceContext<TEntity> _context;
            private IEntitySource<TEntity>[] _sources;

            private TEntity _current;
            private IEnumerator<TEntity>? _currentEnumerator;
            private int _index;

            public Enumerator(EntitySourceContext<TEntity> context, IEntitySource<TEntity>[] sources)
            {
                _buffer = new Dictionary<int, TEntity>(256);
                _context = context;
                _sources = sources;

                _current = null!;
                _currentEnumerator = null;
            }

            public IEnumerator<TEntity> GetEnumerator() => this;

            public bool MoveNext()
            {
                for (; _index < _sources.Length; _index++)
                {
                    var enumerator = _currentEnumerator ??= _sources[_index].GetEntities(_context).GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;

                        if (current == null) continue;

                        _buffer.Add(current.Id, current);
                        _current = current;

                        return true;
                    }
                    
                    _currentEnumerator.Dispose();
                    _sources[_index].Dispose();
                    _currentEnumerator = null;
                }

                return false;
            }

            public bool TryGet(int id, out TEntity entity)
            {
                if (_buffer.TryGetValue(id, out entity)) return true;

                while (MoveNext())
                {
                    if (_current.Id != id) continue;

                    entity = _current;
                    return true;
                }

                return false;
            }

            void IEnumerator.Reset()
            {
            }

            object IEnumerator.Current => Current;
            IEnumerator IEnumerable.GetEnumerator() => this;

            public void Dispose()
            {
                _buffer.Clear();
                _buffer = null!;

                _current = null!;

                _context._enumerator = null;
                _context = null!;

                _sources = null!;

                _currentEnumerator?.Dispose();
                _currentEnumerator = null!;
            }
        }
    }
}