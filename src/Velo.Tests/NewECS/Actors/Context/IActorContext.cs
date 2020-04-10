using System;
using System.Collections.Generic;
using Velo.Tests.NewECS.Actors.Filters;
using Velo.Tests.NewECS.Actors.Groups;
using Velo.Tests.NewECS.Components;

namespace Velo.Tests.NewECS.Actors.Context
{
    public interface IActorContext : IEnumerable<Actor>, IDisposable
    {
        event Action<Actor> Added;

        event Action<Actor, IComponent> ComponentAdded;

        event Action<Actor> Removed;

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

        bool Remove(Actor actor);

        bool TryGet(int actorId, out Actor actor);
    }
}