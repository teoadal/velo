using Velo.ECS.Components;

namespace Velo.ECS.Actors.Factory
{
    public interface IActorBuilder
    {
    }

    public interface IActorBuilder<out TActor> : IActorBuilder
        where TActor : Actor
    {
        TActor Build(int actorId, IComponent[]? components);
    }
}