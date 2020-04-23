using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Velo.Collections;
using Velo.ECS.Actors.Context;
using Velo.ECS.Components;
using Velo.Utils;

namespace Velo.ECS.Actors.Filters
{
    public interface IActorFilter<TComponent> : IActorFilter, IEnumerable<Actor<TComponent>>
        where TComponent : IComponent
    {
        event Action<Actor<TComponent>>? Added;

        event Action<Actor<TComponent>>? Removed;

        bool TryGet(int actorId, out Actor<TComponent> actor);

        IEnumerable<Actor<TComponent>> Where<TArg>(Func<Actor<TComponent>, TArg, bool> filter, TArg arg);
    }

    internal sealed class ActorFilter<TComponent> : IActorFilter<TComponent>, IDisposable
        where TComponent : IComponent
    {
        public event Action<Actor<TComponent>>? Added;

        public event Action<Actor<TComponent>>? Removed;

        public int Length => _actors.Count;

        private readonly List<Actor<TComponent>> _actors;
        private readonly IActorContext _context;
        private readonly ReaderWriterLockSlim _lock;

        public ActorFilter(IActorContext context)
        {
            _actors = new List<Actor<TComponent>>();
            _lock = new ReaderWriterLockSlim();

            context.Added += OnActorAdded;
            context.ComponentAdded += OnActorComponentAdded;
            context.Removed += OnActorRemoved;

            _context = context;
        }

        public bool Contains(int actorId) => TryGet(actorId, out _);

        public IEnumerator<Actor<TComponent>> GetEnumerator()
        {
            return new ReadLockEnumerator<Actor<TComponent>>(_actors, _lock);
        }

        public bool TryGet(int actorId, out Actor<TComponent> actor)
        {
            using (ReadLock.Enter(_lock))
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var exists in _actors)
                {
                    if (exists.Entity.Id != actorId) continue;
                    actor = exists;
                    return true;
                }
            }

            actor = default;
            return false;
        }

        public IEnumerable<Actor<TComponent>> Where<TArg>(Func<Actor<TComponent>, TArg, bool> filter, TArg arg)
        {
            return new ReadLockWhereEnumerator<Actor<TComponent>, TArg>(_actors, filter, arg, _lock);
        }

        private void OnActorAdded(Actor actor)
        {
            if (!actor.TryGetComponent<TComponent>(out var component)) return;

            var wrapper = new Actor<TComponent>(actor, component);
            using (WriteLock.Enter(_lock))
            {
                _actors.Add(wrapper);
            }

            OnWrapperAdded(wrapper);
        }

        private void OnActorComponentAdded(Actor actor, IComponent component)
        {
            if (!(component is TComponent found)) return;

            var wrapper = new Actor<TComponent>(actor, found);
            using (WriteLock.Enter(_lock))
            {
                _actors.Add(wrapper);
            }

            OnWrapperAdded(wrapper);
        }

        private void OnActorComponentRemoved(Actor actor, IComponent component)
        {
            if (actor.ContainsComponent<TComponent>()) return;
            if (TryRemove(actor.Id, out var wrapper))
            {
                OnWrapperRemoved(wrapper);
            }
        }

        private void OnActorRemoved(Actor actor)
        {
            if (!actor.ContainsComponent<TComponent>()) return;

            if (TryRemove(actor.Id, out var wrapper))
            {
                OnWrapperRemoved(wrapper);
            }
        }

        private void OnWrapperAdded(Actor<TComponent> wrapper)
        {
            wrapper.Entity.ComponentRemoved += OnActorComponentRemoved;

            var evt = Added;
            evt?.Invoke(wrapper);
        }

        private void OnWrapperRemoved(Actor<TComponent> wrapper)
        {
            wrapper.Entity.ComponentRemoved -= OnActorComponentRemoved;

            var evt = Removed;
            evt?.Invoke(wrapper);
        }

        private bool TryRemove(int actorId, out Actor<TComponent> wrapper)
        {
            using (WriteLock.Enter(_lock))
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var exists in _actors)
                {
                    if (exists.Entity.Id != actorId) continue;

                    _actors.Remove(exists);

                    wrapper = exists;
                    return true;
                }
            }

            wrapper = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            _context.Added -= OnActorAdded;
            _context.ComponentAdded -= OnActorComponentAdded;
            _context.Removed -= OnActorRemoved;

            _lock.Dispose();
        }
    }
}