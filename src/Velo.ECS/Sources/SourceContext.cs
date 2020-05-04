using System.Collections;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.ECS.Sources
{
    public interface ISourceContext<out TEntity>
        where TEntity : class, IEntity
    {
        TEntity Get(int id);

        IEnumerable<TEntity> GetEntities();
    }

    internal sealed class SourceContext<TEntity> : ISourceContext<TEntity>
        where TEntity : class, IEntity
    {
        private Enumerator? _enumerator;
        private readonly ISource<TEntity>[] _sources;

        public SourceContext(ISource<TEntity>[] sources)
        {
            _sources = sources;
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
            var buffer = new Dictionary<int, TEntity>(256);
            var enumerators = new List<IEnumerator<TEntity>>(_sources.Length);

            foreach (var source in _sources)
            {
                var entities = source.GetEntities(this);

                if (entities is ICollection<TEntity> collection)
                {
                    foreach (var entity in collection)
                    {
                        buffer.Add(entity.Id, entity);
                    }
                }
                else
                {
                    enumerators.Add(entities.GetEnumerator());
                }
            }

            _enumerator = new Enumerator(buffer, enumerators.ToArray());
            return _enumerator;
        }

        private sealed class Enumerator : IEnumerable<TEntity>, IEnumerator<TEntity>
        {
            public TEntity Current => _current;

            private Dictionary<int, TEntity> _buffer;
            private TEntity _current;
            private IEnumerator<TEntity>[] _enumerators;
            private int _index;

            public Enumerator(Dictionary<int, TEntity> buffer, IEnumerator<TEntity>[] enumerators)
            {
                _buffer = buffer;
                _current = null!;
                _enumerators = enumerators;
            }

            public IEnumerator<TEntity> GetEnumerator() => this;

            public bool MoveNext()
            {
                var enumerator = _enumerators[_index]; // TODO: save between iteration 

                while (_index < _enumerators.Length)
                {
                    while (enumerator.MoveNext())
                    {
                        _current = enumerator.Current;
                        return true;
                    }

                    _index++;
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
                foreach (var enumerator in _enumerators)
                {
                    enumerator.Dispose();
                }

                _buffer.Clear();

                _buffer = null!;
                _current = null!;
                _enumerators = null!;
            }
        }
    }
}