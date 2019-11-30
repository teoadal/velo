using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.ECS.Enumeration;
using Velo.Utils;

namespace Velo.ECS.Actors
{
    public abstract class ActorFilter: EntityFilter<Actor>
    {
        public event Action<Actor> Added;

        public event Action<Actor> Removed;

        protected ActorFilter(params int[] componentTypeIds) : base(componentTypeIds)
        {
        }

        protected override void OnAdded(Actor actor)
        {
            actor.RemovedComponent += OnComponentRemoved;

            var evt = Added;
            evt?.Invoke(actor);
        }

        internal void OnComponentAdded(Actor actor)
        {
            OnAddedToContext(actor);
        }

        private void OnComponentRemoved(Actor actor, IComponent component)
        {
            if (Applicable(actor)) return;

            OnRemoved(actor);
        }

        internal void OnRemovedFromContext(Actor actor)
        {
            if (!Applicable(actor)) return;

            OnRemoved(actor);
        }

        protected abstract bool Remove(Actor actor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnRemoved(Actor actor)
        {
            if (!Remove(actor)) return;

            actor.RemovedComponent -= OnComponentRemoved;

            var evt = Removed;
            evt?.Invoke(actor);
        }
    }

    public sealed class ActorFilter<TComponent1> : ActorFilter
        where TComponent1 : IComponent
    {
        public int Length => _wrappers.Count;
        
        private readonly List<Wrapper<Actor, TComponent1>> _wrappers;

        public ActorFilter() : base(Typeof<TComponent1>.Id)
        {
            _wrappers = new List<Wrapper<Actor, TComponent1>>();
        }

        public override bool Contains(Actor entity)
        {
            foreach (var wrapper in _wrappers)
            {
                if (wrapper.Entity.Equals(entity)) return true;
            }

            return false;
        }

        public Wrapper<Actor, TComponent1> Get(int id)
        {
            foreach (var wrapper in _wrappers)
            {
                if (wrapper.Entity.Id == id)
                {
                    return wrapper;
                }
            }

            throw Error.NotFound($"Entity with id {id} not found");
        }

        public List<Wrapper<Actor, TComponent1>>.Enumerator GetEnumerator() => _wrappers.GetEnumerator();

        public WhereFilter<Actor, TComponent1> Where(Predicate<Wrapper<Actor, TComponent1>> predicate)
        {
            return new WhereFilter<Actor, TComponent1>(_wrappers.GetEnumerator(), predicate);
        }

        protected override bool Add(Actor actor)
        {
            _wrappers.Add(new Wrapper<Actor, TComponent1>(actor, actor.GetComponent<TComponent1>()));

            return true;
        }

        protected override bool Remove(Actor actor)
        {
            var actorId = actor.Id;
            foreach (var wrapper in _wrappers)
            {
                if (wrapper.Entity.Id != actorId) continue;
                return _wrappers.Remove(wrapper);
            }

            return false;
        }
    }

    public sealed class ActorFilter<TComponent1, TComponent2> : ActorFilter
        where TComponent1 : IComponent where TComponent2 : IComponent
    {
        public int Length => _wrappers.Count;
        
        private readonly List<Wrapper<Actor, TComponent1, TComponent2>> _wrappers;

        public ActorFilter() : base(Typeof<TComponent1>.Id, Typeof<TComponent2>.Id)
        {
            _wrappers = new List<Wrapper<Actor, TComponent1, TComponent2>>();
        }

        public override bool Contains(Actor entity)
        {
            foreach (var wrapper in _wrappers)
            {
                if (wrapper.Entity.Equals(entity)) return true;
            }

            return false;
        }

        public List<Wrapper<Actor, TComponent1, TComponent2>>.Enumerator GetEnumerator() => _wrappers.GetEnumerator();

        public WhereFilter<Actor, TComponent1, TComponent2> Where(
            Predicate<Wrapper<Actor, TComponent1, TComponent2>> predicate)
        {
            return new WhereFilter<Actor, TComponent1, TComponent2>(_wrappers.GetEnumerator(), predicate);
        }

        protected override bool Add(Actor actor)
        {
            _wrappers.Add(new Wrapper<Actor, TComponent1, TComponent2>(
                actor,
                actor.GetComponent<TComponent1>(),
                actor.GetComponent<TComponent2>()));

            return true;
        }

        protected override bool Remove(Actor actor)
        {
            var actorId = actor.Id;
            foreach (var wrapper in _wrappers)
            {
                if (wrapper.Entity.Id != actorId) continue;
                return _wrappers.Remove(wrapper);
            }

            return false;
        }
    }
}