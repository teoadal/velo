using System.Collections.Generic;
using Velo.ECS.Components;

namespace Velo.ECS.Actors.Factory
{
    public ref struct ActorConfigurator
    {
        private readonly IActorFactory _actorFactory;
        private readonly IComponentFactory _componentFactory;

        private readonly List<IComponent> _components;
        private int? _id;

        internal ActorConfigurator(IActorFactory actorFactory, IComponentFactory componentFactory)
        {
            _componentFactory = componentFactory;
            _actorFactory = actorFactory;
            _components = new List<IComponent>();
            _id = null;
        }

        public readonly ActorConfigurator AddComponent(IComponent component)
        {
            _components.Add(component);
            return this;
        }

        public readonly ActorConfigurator AddComponent<TComponent>() where TComponent : IComponent
        {
            _components.Add(_componentFactory.Create<TComponent>());
            return this;
        }

        public readonly Actor Build()
        {
            return _actorFactory.Create(_components.ToArray(), _id);
        }

        public readonly TActor Build<TActor>() where TActor : Actor
        {
            return _actorFactory.Create<TActor>(_components.ToArray(), _id);
        }

        public ActorConfigurator SetId(int actorId)
        {
            _id = actorId;
            return this;
        }
    }
}