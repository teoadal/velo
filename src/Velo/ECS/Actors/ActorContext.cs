using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.ECS.Actors
{
    public sealed class ActorContext : EntityContext<Actor>
    {
        public event Action<Actor> Removed;

        private readonly Dictionary<int, ActorFilter> _filters;
        private readonly Dictionary<int, IActorGroup> _groups;

        public ActorContext()
        {
            _filters = new Dictionary<int, ActorFilter>();
            _groups = new Dictionary<int, IActorGroup>();
        }

        public ActorFilter<TComponent1> GetFilter<TComponent1>() where TComponent1 : IComponent
        {
            var filterId = Typeof<ActorFilter<TComponent1>>.Id;

            if (!_filters.TryGetValue(filterId, out var filter))
            {
                filter = new ActorFilter<TComponent1>();
                _filters.Add(filterId, filter);
            }

            return (ActorFilter<TComponent1>) filter;
        }

        public ActorFilter<TComponent1, TComponent2> GetFilter<TComponent1, TComponent2>()
            where TComponent1 : IComponent where TComponent2 : IComponent
        {
            var filterId = Typeof<ActorFilter<TComponent1, TComponent2>>.Id;

            if (!_filters.TryGetValue(filterId, out var filter))
            {
                filter = new ActorFilter<TComponent1, TComponent2>();
                filter.Initialize(this);

                _filters.Add(filterId, filter);
            }

            return (ActorFilter<TComponent1, TComponent2>) filter;
        }
        
        public ActorGroup<TActor> GetGroup<TActor>() where TActor: Actor
        {
            var groupId = Typeof<TActor>.Id;

            if (!_groups.TryGetValue(groupId, out var assetGroup))
            {
                assetGroup = new ActorGroup<TActor>();
                assetGroup.Initialize(this);
                
                _groups.Add(groupId, assetGroup);
            }

            return (ActorGroup<TActor>) assetGroup;
        }
        
        public bool Remove(Actor actor)
        {
            if (!TryRemove(actor)) return false;

            OnRemoved(actor);
            return true;
        }
        
        protected override void OnAdded(Actor actor)
        {
            actor.AddedComponent += OnAddedComponent;

            foreach (var filter in _filters.Values)
            {
                filter.OnAddedToContext(actor);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OnRemoved(Actor actor)
        {
            actor.AddedComponent -= OnAddedComponent;

            foreach (var filter in _filters.Values)
            {
                filter.OnRemovedFromContext(actor);
            }

            var evt = Removed;
            evt?.Invoke(actor);
        }
        
        private void OnAddedComponent(Actor actor, IComponent component)
        {
            foreach (var filter in _filters.Values)
            {
                filter.OnComponentAdded(actor);
            }
        }
    }
}