using System;
using System.Collections.Generic;
using Velo.ECS.Enumeration;

namespace Velo.ECS
{
    public abstract class EntityContext<TEntity>
        where TEntity: Entity
    {
        public event Action<TEntity> Added;

        public int Length => _entities.Count;
        
        private readonly Dictionary<int, TEntity> _entities;

        protected EntityContext()
        {
            _entities = new Dictionary<int, TEntity>();
        }

        public void Add(TEntity entity)
        {
            _entities.Add(entity.Id, entity);

            OnAdded(entity);
            
            var evt = Added;
            evt?.Invoke(entity);
        }

        public bool Contains(Predicate<TEntity> predicate)
        {
            foreach (var entity in _entities.Values)
            {
                if (predicate(entity)) 
                    return true;
            }

            return false;
        }
        
        public bool Contains(TEntity entity)
        {
            return _entities.ContainsValue(entity);
        }

        public TEntity Get(int id) => _entities[id];
        
        public Dictionary<int, TEntity>.ValueCollection.Enumerator GetEnumerator()
        {
            return _entities.Values.GetEnumerator();
        }

        public WhereContext<TEntity> Where(Predicate<TEntity> predicate)
        {
            return new WhereContext<TEntity>(_entities.Values.GetEnumerator(), predicate);
        }
        
        protected abstract void OnAdded(TEntity entity);
        
        protected bool TryRemove(TEntity entity)
        {
            return _entities.Remove(entity.Id);
        }
    }
}