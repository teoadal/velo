using System;
using System.Collections.Generic;
using System.Threading;
using Velo.Tests.NewECS.Actors.Context;
using Velo.Tests.NewECS.Components;
using Velo.Utils;

namespace Velo.Tests.NewECS.Actors.Factory
{
    public sealed class ActorFactory : IActorFactory
    {
        private readonly IActorContext _actorContext;
        private readonly IComponentFactory _componentFactory;

        private readonly IActorBuilder[] _builders;
        private readonly Dictionary<int, IActorBuilder> _resolvedBuilders;
        private readonly object _lock;

        private readonly List<int> _freeIds;
        private int _nextId;

        public ActorFactory(IActorContext actorContext, IComponentFactory componentFactory,
            IActorBuilder[] actorBuilders = null)
        {
            _builders = actorBuilders ?? Array.Empty<IActorBuilder>();
            _actorContext = actorContext;
            _componentFactory = componentFactory;
            _resolvedBuilders = new Dictionary<int, IActorBuilder>();
            _lock = new object();

            _freeIds = new List<int>();
            _nextId = 0;
        }

        public ActorConfigurator Configure()
        {
            return new ActorConfigurator(this, _componentFactory);
        }

        public Actor Create(IComponent[] components = null, int? id = null)
        {
            var actorId = CreateOrCheckId(id);
            return new Actor(actorId, components);
        }

        public TActor Create<TActor>(IComponent[] components = null, int? id = null) where TActor : Actor
        {
            var typeId = Typeof<TActor>.Id;

            if (!_resolvedBuilders.TryGetValue(typeId, out var actorBuilder))
            {
                actorBuilder = FindOrCreateBuilder<TActor>();

                using (Lock.Enter(_lock))
                {
                    _resolvedBuilders[typeId] = actorBuilder;
                }
            }

            var actorId = CreateOrCheckId(id);
            return ((IActorBuilder<TActor>) actorBuilder).Build(actorId, components);
        }

        private int CreateOrCheckId(int? id)
        {
            if (id.HasValue)
            {
                var idValue = id.Value;

                if (_actorContext.Contains(idValue))
                {
                    throw Error.AlreadyExists($"Actor with id {idValue} already exists");
                }

                using (Lock.Enter(_freeIds))
                {
                    _freeIds.Remove(idValue);
                }

                return idValue;
            }

            var actorId = Interlocked.Increment(ref _nextId);
            while (_actorContext.Contains(actorId))
                actorId = Interlocked.Increment(ref _nextId);

            return actorId;
        }

        private IActorBuilder FindOrCreateBuilder<TActor>() where TActor : Actor
        {
            foreach (var builder in _builders)
            {
                if (builder is IActorBuilder<TActor>)
                {
                    return builder;
                }
            }

            throw Error.NotFound($"Actor builder for type {ReflectionUtils.GetName<TActor>()} isn't registered");
        }
    }
}