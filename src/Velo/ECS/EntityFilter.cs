using System.Runtime.CompilerServices;

namespace Velo.ECS
{
    public abstract class EntityFilter<TEntity>
        where TEntity : Entity
    {
        private readonly int[] _componentTypeIds;

        protected EntityFilter(int[] componentTypeIds)
        {
            _componentTypeIds = componentTypeIds;
        }

        public abstract bool Contains(TEntity entity);

        internal void Initialize(EntityContext<TEntity> context)
        {
            foreach (var entity in context)
            {
                OnAddedToContext(entity);
            }
        }

        internal void OnAddedToContext(TEntity entity)
        {
            if (!Applicable(entity)) return;

            if (Add(entity)) OnAdded(entity);
        }

        protected abstract bool Add(TEntity entity);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool Applicable(TEntity entity)
        {
            ref readonly var entitySign = ref entity.Sign;
            return entitySign.ContainsAll(_componentTypeIds);
        }

        protected abstract void OnAdded(TEntity entity);
    }
}