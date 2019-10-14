using System;
using System.Collections;
using System.Collections.Generic;

namespace Velo.ECS.Enumeration
{
    public struct WhereContext<TEntity> : IEnumerator<TEntity>, IEnumerable<TEntity>
    {
        public TEntity Current { get; private set; }
        
        private Dictionary<int, TEntity>.ValueCollection.Enumerator _enumerator;
        private Predicate<TEntity> _predicate;

        public WhereContext(Dictionary<int, TEntity>.ValueCollection.Enumerator enumerator, Predicate<TEntity> predicate)
        {
            _enumerator = enumerator;
            _predicate = predicate;

            Current = default;
        }

        public IEnumerator<TEntity> GetEnumerator() => this;
        
        public bool MoveNext()
        {
            while (_enumerator.MoveNext())
            {
                var item = _enumerator.Current;

                if (!_predicate(item)) continue;

                Current = item;
                return true;
            }

            return false;
        }


        public void Dispose()
        {
            _enumerator.Dispose();
            _predicate = null;
        }

        object IEnumerator.Current => Current;
        
        void IEnumerator.Reset()
        {
        }
        
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}