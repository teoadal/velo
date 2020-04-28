using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Velo.Collections;
using Velo.ECS.Actors.Filters;
using Velo.ECS.Actors.Groups;
using Velo.ECS.Components;
using Velo.Threading;
using Velo.Utils;

namespace Velo.ECS.Actors.Context
{
    internal sealed class ActorContext : IActorContext
    {
        public event Action<Actor>? Added;

        public event Action<Actor, IComponent>? ComponentAdded;

        public event Action<Actor>? Removed;

        private readonly Dictionary<int, Actor> _actors;
        private readonly ReaderWriterLockSlim _actorsLock;
        private readonly Dictionary<int, IActorFilter> _filters;
        private readonly Dictionary<int, IActorGroup> _groups;
        private readonly Dictionary<int, object> _singleActors;

        public ActorContext()
        {
            _actorsLock = new ReaderWriterLockSlim();

            _actors = new Dictionary<int, Actor>(1000);
            _filters = new Dictionary<int, IActorFilter>(50);
            _groups = new Dictionary<int, IActorGroup>(50);
            _singleActors = new Dictionary<int, object>();
        }

        public void Add(Actor actor)
        {
            using (WriteLock.Enter(_actorsLock))
            {
                _actors.Add(actor.Id, actor);
            }

            Subscribe(actor);
        }

        public void AddRange(params Actor[] actors)
        {
            using (WriteLock.Enter(_actorsLock))
            {
                foreach (var actor in actors)
                {
                    _actors.Add(actor.Id, actor);
                }
            }

            foreach (var actor in actors)
            {
                Subscribe(actor);
            }
        }

        public void AddFilter<TComponent>(IActorFilter<TComponent> actorFilter) where TComponent : IComponent
        {
            var filterId = Typeof<IActorFilter<TComponent>>.Id;
            using (Lock.Enter(_filters))
            {
                _filters.Add(filterId, actorFilter);
            }
        }

        public void AddFilter<TComponent1, TComponent2>(IActorFilter<TComponent1, TComponent2> actorFilter)
            where TComponent1 : IComponent where TComponent2 : IComponent
        {
            var filterId = Typeof<IActorFilter<TComponent1, TComponent2>>.Id;
            using (Lock.Enter(_filters))
            {
                _filters.Add(filterId, actorFilter);
            }
        }

        public void AddGroup<TActor>(IActorGroup<TActor> actorGroup) where TActor : Actor
        {
            var typeId = Typeof<TActor>.Id;

            using (Lock.Enter(_groups))
            {
                _groups.Add(typeId, actorGroup);
            }
        }

        public void Clear()
        {
            using (WriteLock.Enter(_actorsLock))
            {
                foreach (var actor in _actors.Values)
                {
                    Unsubscribe(actor);
                }

                _actors.Clear();
            }
        }

        public bool Contains(int actorId)
        {
            using (ReadLock.Enter(_actorsLock))
            {
                return _actors.ContainsKey(actorId);
            }
        }

        public Actor Get(int actorId)
        {
            return TryGet(actorId, out var actor)
                ? actor
                : throw Error.NotFound($"Actor with id '{actorId}' not found in context");
        }

        public IEnumerator<Actor> GetEnumerator()
        {
            var valueCollection = _actors.Values;
            return new ReadLockEnumerator<int, Actor>(valueCollection, _actorsLock);
        }

        public IActorFilter<TComponent> GetFilter<TComponent>() where TComponent : IComponent
        {
            var filterId = Typeof<IActorFilter<TComponent>>.Id;

            IActorFilter filter;
            using (Lock.Enter(_filters))
            {
                // ReSharper disable once InvertIf
                if (!_filters.TryGetValue(filterId, out filter))
                {
                    filter = new ActorFilter<TComponent>(this);
                    _filters.Add(filterId, filter);
                }
            }

            return (IActorFilter<TComponent>) filter;
        }

        public IActorFilter<TComponent1, TComponent2> GetFilter<TComponent1, TComponent2>()
            where TComponent1 : IComponent where TComponent2 : IComponent
        {
            var filterId = Typeof<IActorFilter<TComponent1, TComponent2>>.Id;

            IActorFilter filter;
            using (Lock.Enter(_filters))
            {
                // ReSharper disable once InvertIf
                if (!_filters.TryGetValue(filterId, out filter))
                {
                    filter = new ActorFilter<TComponent1, TComponent2>(this);
                    _filters.Add(filterId, filter);
                }
            }

            return (IActorFilter<TComponent1, TComponent2>) filter;
        }

        public IActorGroup<TActor> GetGroup<TActor>() where TActor : Actor
        {
            var actorTypeId = Typeof<TActor>.Id;

            IActorGroup actorGroup;
            using (Lock.Enter(_groups))
            {
                // ReSharper disable once InvertIf
                if (!_groups.TryGetValue(actorTypeId, out actorGroup))
                {
                    actorGroup = new ActorGroup<TActor>(this);
                    _groups.Add(actorTypeId, actorGroup);
                }
            }

            return (IActorGroup<TActor>) actorGroup;
        }

        public SingleActor<TActor> GetSingle<TActor>() where TActor : Actor
        {
            var typeId = Typeof<TActor>.Id;

            object singleActor;
            using (Lock.Enter(_singleActors))
            {
                // ReSharper disable once InvertIf
                if (!_singleActors.TryGetValue(typeId, out singleActor))
                {
                    singleActor = new SingleActor<TActor>(this);
                    _singleActors.Add(typeId, singleActor);
                }
            }

            return (SingleActor<TActor>) singleActor;
        }

        public bool Remove(Actor actor)
        {
            bool removeResult;
            using (WriteLock.Enter(_actorsLock))
            {
                removeResult = _actors.Remove(actor.Id);
            }

            if (removeResult)
            {
                Unsubscribe(actor);
            }

            return removeResult;
        }

        public bool TryGet(int actorId, out Actor actor)
        {
            bool result;
            using (ReadLock.Enter(_actorsLock))
            {
                result = _actors.TryGetValue(actorId, out actor);
            }

            return result;
        }

        public IEnumerable<Actor> Where<TArg>(Func<Actor, TArg, bool> filter, TArg arg)
        {
            return new ReadLockWhereEnumerator<int, Actor, TArg>(_actors.Values, filter, arg, _actorsLock);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Subscribe(Actor actor)
        {
            actor.ComponentAdded += ComponentAdded;

            var evt = Added;
            evt?.Invoke(actor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Unsubscribe(Actor actor)
        {
            actor.ComponentAdded -= ComponentAdded;

            var evt = Removed;
            evt?.Invoke(actor);
        }

        public void Dispose()
        {
            Clear();

            CollectionUtils.DisposeValuesIfDisposable(_filters);
            CollectionUtils.DisposeValuesIfDisposable(_groups);
            CollectionUtils.DisposeValuesIfDisposable(_singleActors);

            _actorsLock.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}