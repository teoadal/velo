using Velo.Tests.NewECS.Components;

namespace Velo.Tests.NewECS.Actors.Factory
{
    public interface IActorBuilder
    {
    }

    public interface IActorBuilder<out TActor> : IActorBuilder
        where TActor : Actor
    {
        TActor Build(int actorId, IComponent[] components);
    }
}