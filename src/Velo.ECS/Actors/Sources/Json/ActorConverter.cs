using System;
using Velo.ECS.Actors.Factory;
using Velo.ECS.Components;
using Velo.ECS.Sources.Json.Objects;

namespace Velo.ECS.Actors.Sources.Json
{
    internal sealed class ActorConverter<TActor> : EntityConverter<TActor>
        where TActor : Actor
    {
        private readonly IActorFactory _actorFactory;

        public ActorConverter(IActorFactory actorFactory, IServiceProvider services)
            : base(services, typeof(TActor) != typeof(Actor))
        {
            _actorFactory = actorFactory;
        }

        protected override TActor CreateEntity(Type? actorType, int id, IComponent[]? components)
        {
            var actor = actorType == null
                ? _actorFactory.Create(components, id)
                : _actorFactory.Create(actorType, components, id);

            return (TActor) actor;
        }
    }
}