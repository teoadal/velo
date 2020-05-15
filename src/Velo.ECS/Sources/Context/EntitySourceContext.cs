using System.Collections.Generic;
using Velo.Collections.Enumerators;
using Velo.Utils;

namespace Velo.ECS.Sources.Context
{
    internal sealed partial class EntitySourceContext<TEntity> : IEntitySourceContext<TEntity>
        where TEntity : class, IEntity
    {
        public bool IsStarted => _enumerator != null;

        private Enumerator? _enumerator;
        private readonly IReference<IEntitySource<TEntity>[]> _sources;

        public EntitySourceContext(IReference<IEntitySource<TEntity>[]> sources)
        {
            _sources = sources;
            _enumerator = null!;
        }

        public EntitySourceContext(params IEntitySource<TEntity>[] sources)
            : this(new MemorySources(sources))
        {
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

            var sources = _sources.Value;
            if (sources == null || sources.Length == 0)
            {
                return EmptyEnumerator<TEntity>.Instance;
            }

            _enumerator = new Enumerator(this, sources);
            return _enumerator;
        }
    }
}