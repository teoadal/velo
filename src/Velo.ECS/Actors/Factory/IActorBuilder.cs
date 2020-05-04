using Velo.ECS.Components;
using Velo.ECS.Factory;

namespace Velo.ECS.Actors.Factory
{
    public interface IActorBuilder
    {
        Actor BuildActor(int actorId, IComponent[]? components);
    }

    public interface IActorBuilder<out TActor> : IActorBuilder
        where TActor : Actor
    {
        TActor Build(int actorId, IComponent[]? components);
    }

    internal sealed class DefaultActorBuilder<TActor> : DefaultEntityBuilder<TActor>, IActorBuilder<TActor>
        where TActor : Actor
    {
        public Actor BuildActor(int actorId, IComponent[]? components)
        {
            return Build(actorId, components);
        }
    }
}