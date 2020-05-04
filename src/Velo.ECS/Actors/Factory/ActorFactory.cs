using System;
using System.Collections.Generic;
using System.Threading;
using Velo.Collections;
using Velo.ECS.Actors.Context;
using Velo.ECS.Components;
using Velo.Threading;
using Velo.Utils;

namespace Velo.ECS.Actors.Factory
{
    internal sealed class ActorFactory : DangerousVector<int, IActorBuilder>, IActorFactory
    {
        private readonly IActorContext _actorContext;
        private readonly IActorBuilder[] _builders;
        private readonly IComponentFactory _componentFactory;

        private readonly Func<int, Type, IActorBuilder> _findOrCreate;

        private readonly List<int> _freeIds;
        private int _nextId;

        public ActorFactory(IActorContext actorContext, IComponentFactory componentFactory,
            IActorBuilder[]? actorBuilders = null)
        {
            _actorContext = actorContext;
            _builders = actorBuilders ?? Array.Empty<IActorBuilder>();
            _componentFactory = componentFactory;

            _findOrCreate = FindOrCreateBuilder;

            _freeIds = new List<int>();
            _nextId = 0;
        }

        public ActorConfigurator Configure()
        {
            return new ActorConfigurator(this, _componentFactory);
        }

        public Actor Create(IComponent[]? components = null, int? actorId = null)
        {
            var id = CreateOrCheckId(actorId);
            return new Actor(id, components);
        }

        public Actor Create(Type actorType, IComponent[]? components = null, int? actorId = null)
        {
            var typeId = Typeof.GetTypeId(actorType);
            var builder = GetOrAdd(typeId, _findOrCreate, actorType);

            var id = CreateOrCheckId(actorId);
            return builder.BuildActor(id, components);
        }

        public TActor Create<TActor>(IComponent[]? components = null, int? actorId = null) where TActor : Actor
        {
            var typeId = Typeof<TActor>.Id;
            var builder = (IActorBuilder<TActor>) GetOrAdd(typeId, _findOrCreate, Typeof<TActor>.Raw);

            var id = CreateOrCheckId(actorId);
            return builder.Build(id, components);
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

        private IActorBuilder FindOrCreateBuilder(int _, Type actorType)
        {
            var builderType = typeof(IActorBuilder<>).MakeGenericType(actorType);

            foreach (var builder in _builders)
            {
                if (builderType.IsInstanceOfType(builder))
                {
                    return builder;
                }
            }

            var instanceType = typeof(DefaultActorBuilder<>).MakeGenericType(actorType);
            var instance = Activator.CreateInstance(instanceType);

            return (IActorBuilder) instance;
        }
    }
}