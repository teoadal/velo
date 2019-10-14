using System;

namespace Velo.ECS.Actors
{
    public sealed class ActorGroup<TActor>: EntityGroup<TActor>, IActorGroup
        where TActor: Actor
    {
        public event Action<TActor> Removed; 
        
        internal ActorGroup()
        {
        }
        
        void IActorGroup.Initialize(ActorContext context)
        {
            foreach (var actor in context)
            {
                if (actor is TActor found)
                {
                    Add(found);
                }
            }
        }

        void IActorGroup.OnAddedToContext(Actor actor)
        {
            if (actor is TActor found)
            {
                Add(found);
            }
        }

        void IActorGroup.OnRemovedFromContext(Actor actor)
        {
            if (!(actor is TActor found) || !Remove(found)) return;
            
            var evt = Removed;
            evt?.Invoke(found);
        }
    }

    internal interface IActorGroup
    {
        void Initialize(ActorContext context);

        void OnAddedToContext(Actor actor);

        void OnRemovedFromContext(Actor actor);
    }
}