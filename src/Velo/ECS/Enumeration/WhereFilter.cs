using System;
using System.Collections.Generic;

namespace Velo.ECS.Enumeration
{
    public ref struct WhereFilter<TEntity, TComponent1>
        where TEntity : Entity where TComponent1 : IComponent
    {
        public Wrapper<TEntity, TComponent1> Current { get; private set; }

        private List<Wrapper<TEntity, TComponent1>>.Enumerator _enumerator;
        private Predicate<Wrapper<TEntity, TComponent1>> _predicate;

        internal WhereFilter(List<Wrapper<TEntity, TComponent1>>.Enumerator enumerator,
            Predicate<Wrapper<TEntity, TComponent1>> predicate)
        {
            _predicate = predicate;
            _enumerator = enumerator;

            Current = default;
        }

        public WhereFilter<TEntity, TComponent1> GetEnumerator() => this;
        
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
    }

    public ref struct WhereFilter<TEntity, TComponent1, TComponent2>
        where TEntity : Entity where TComponent1 : IComponent where TComponent2 : IComponent
    {
        public Wrapper<TEntity, TComponent1, TComponent2> Current { get; private set; }

        private List<Wrapper<TEntity, TComponent1, TComponent2>>.Enumerator _enumerator;
        private Predicate<Wrapper<TEntity, TComponent1, TComponent2>> _predicate;

        internal WhereFilter(List<Wrapper<TEntity, TComponent1, TComponent2>>.Enumerator enumerator,
            Predicate<Wrapper<TEntity, TComponent1, TComponent2>> predicate)
        {
            _predicate = predicate;
            _enumerator = enumerator;

            Current = default;
        }

        public WhereFilter<TEntity, TComponent1, TComponent2> GetEnumerator() => this;
        
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
    }
}