using System;
using System.Collections.Generic;

namespace Velo.Tests.NewECS.Actors.Groups
{
    public interface IActorGroup
    {
        int Length { get; }

        bool Contains(int actorId);
    }

    public interface IActorGroup<TActor> : IActorGroup, IEnumerable<TActor>
        where TActor : Actor
    {
        event Action<TActor> Added;

        event Action<TActor> Removed;

        bool TryGet(int actorId, out TActor actor);

        IEnumerable<TActor> Where<TArg>(Func<TActor, TArg, bool> filter, TArg arg);
    }
}