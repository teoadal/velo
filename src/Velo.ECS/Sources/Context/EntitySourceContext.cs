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

        public EntitySourceContext()
        {
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

        public IEnumerable<TEntity> GetEntities(IEntitySource<TEntity>[] sources)
        {
            if (_enumerator != null)
            {
                throw Error.InvalidOperation("Entities context already started");
            }

            if (sources.Length == 0)
            {
                return EmptyEnumerator<TEntity>.Instance;
            }

            _enumerator = new Enumerator(this, sources);
            return _enumerator;
        }
    }
}