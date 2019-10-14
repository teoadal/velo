using System;
using System.Collections.Generic;
using Velo.ECS.Enumeration;

namespace Velo.ECS
{
    public abstract class EntityGroup<TEntity>
        where TEntity: Entity
    {
        public event Action<TEntity> Added; 
        
        private readonly Dictionary<int, TEntity> _entities;

        protected EntityGroup()
        {
            _entities = new Dictionary<int, TEntity>();
        }

        public TEntity Get(int id) => _entities[id];
        
        public Dictionary<int, TEntity>.ValueCollection.Enumerator GetEnumerator() => _entities.Values.GetEnumerator();

        public WhereContext<TEntity> Where(Predicate<TEntity> predicate)
        {
            return new WhereContext<TEntity>(_entities.Values.GetEnumerator(), predicate);
        }
        
        protected void Add(TEntity entity)
        {
            _entities.Add(entity.Id, entity);

            var evt = Added;
            evt?.Invoke(entity);
        }
        
        protected bool Remove(TEntity entity)
        {
            return _entities.Remove(entity.Id);
        }
    }
}