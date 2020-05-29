using System;
using System.Collections.Generic;
using Velo.ECS.Actors.Filters;
using Velo.ECS.Actors.Groups;
using Velo.ECS.Components;
using Velo.ECS.Sources;
using Velo.ECS.Stores;

namespace Velo.ECS.Actors.Context
{
    public interface IActorContext : IEnumerable<Actor>, IDisposable
    {
        event Action<Actor>? Added;

        event Action<Actor, IComponent>? ComponentAdded;

        event Action<Actor>? Removed;
        
        int Length { get; }

        void Add(Actor actor);

        void AddRange(params Actor[] actors);

        void AddFilter<TComponent>(IActorFilter<TComponent> actorFilter) where TComponent : IComponent;

        void AddFilter<TComponent1, TComponent2>(IActorFilter<TComponent1, TComponent2> actorFilter)
            where TComponent1 : IComponent where TComponent2 : IComponent;

        void AddGroup<TActor>(IActorGroup<TActor> actorGroup) where TActor : Actor;

        void Clear();

        bool Contains(int actorId);

        Actor Get(int actorId);

        IActorFilter<TComponent> GetFilter<TComponent>() where TComponent : IComponent;

        IActorFilter<TComponent1, TComponent2> GetFilter<TComponent1, TComponent2>()
            where TComponent1 : IComponent where TComponent2 : IComponent;

        IActorGroup<TActor> GetGroup<TActor>() where TActor : Actor;

        SingleActor<TActor> GetSingle<TActor>() where TActor : Actor;

        void Load(params IEntitySource<Actor>[] actorSources);

        bool Remove(Actor actor);

        void Save(IEntityStore<Actor> actorStore);
        
        bool TryGet(int actorId, out Actor actor);

        IEnumerable<Actor> Where<TArg>(Func<Actor, TArg, bool> filter, TArg arg);
    }
}