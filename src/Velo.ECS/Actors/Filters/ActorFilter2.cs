using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Velo.Collections;
using Velo.ECS.Actors.Context;
using Velo.ECS.Components;
using Velo.Threading;

namespace Velo.ECS.Actors.Filters
{
    public interface IActorFilter<TComponent1, TComponent2> : IActorFilter, IEnumerable<Actor<TComponent1, TComponent2>>
        where TComponent1 : IComponent where TComponent2 : IComponent
    {
        event Action<Actor<TComponent1, TComponent2>>? Added;

        event Action<Actor<TComponent1, TComponent2>>? Removed;

        bool TryGet(int actorId, out Actor<TComponent1, TComponent2> actor);

        IEnumerable<Actor<TComponent1, TComponent2>> Where<TArg>(
            Func<Actor<TComponent1, TComponent2>, TArg, bool> filter, TArg arg);
    }

    internal sealed class ActorFilter<TComponent1, TComponent2> : IActorFilter<TComponent1, TComponent2>, IDisposable
        where TComponent1 : IComponent where TComponent2 : IComponent
    {
        public event Action<Actor<TComponent1, TComponent2>>? Added;
        public event Action<Actor<TComponent1, TComponent2>>? Removed;

        public int Length => _actors.Count;

        private readonly List<Actor<TComponent1, TComponent2>> _actors;
        private readonly IActorContext _context;
        private readonly ReaderWriterLockSlim _lock;

        public ActorFilter(IActorContext context)
        {
            _actors = new List<Actor<TComponent1, TComponent2>>();
            _lock = new ReaderWriterLockSlim();

            context.Added += OnActorAdded;
            context.ComponentAdded += OnActorComponentAdded;
            context.Removed += OnActorRemoved;

            _context = context;
        }

        public bool Contains(int actorId) => TryGet(actorId, out _);

        public IEnumerator<Actor<TComponent1, TComponent2>> GetEnumerator()
        {
            return new ReadLockEnumerator<Actor<TComponent1, TComponent2>>(_actors, _lock);
        }

        public bool TryGet(int actorId, out Actor<TComponent1, TComponent2> actor)
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

        public IEnumerable<Actor<TComponent1, TComponent2>> Where<TArg>(
            Func<Actor<TComponent1, TComponent2>, TArg, bool> filter, TArg arg)
        {
            return new ReadLockWhereEnumerator<Actor<TComponent1, TComponent2>, TArg>(_actors, filter, arg, _lock);
        }

        private void OnActorAdded(Actor actor)
        {
            if (!actor.TryGetComponents<TComponent1, TComponent2>(out var component1, out var component2)) return;

            var wrapper = new Actor<TComponent1, TComponent2>(actor, component1, component2);
            using (WriteLock.Enter(_lock))
            {
                _actors.Add(wrapper);
            }

            OnWrapperAdded(wrapper);
        }

        private void OnActorComponentAdded(Actor actor, IComponent component)
        {
            if (component is TComponent1 || component is TComponent2)
            {
                OnActorAdded(actor);
            }
        }

        private void OnActorComponentRemoved(Actor actor, IComponent component)
        {
            if (actor.ContainsComponent<TComponent2>()) return;
            if (TryRemove(actor.Id, out var wrapper))
            {
                OnWrapperRemoved(wrapper);
            }
        }

        private void OnActorRemoved(Actor actor)
        {
            if (!actor.ContainsComponents<TComponent1, TComponent2>()) return;

            if (TryRemove(actor.Id, out var wrapper))
            {
                OnWrapperRemoved(wrapper);
            }
        }

        private void OnWrapperAdded(Actor<TComponent1, TComponent2> wrapper)
        {
            wrapper.Entity.ComponentRemoved += OnActorComponentRemoved;

            var evt = Added;
            evt?.Invoke(wrapper);
        }

        private void OnWrapperRemoved(Actor<TComponent1, TComponent2> wrapper)
        {
            wrapper.Entity.ComponentRemoved -= OnActorComponentRemoved;

            var evt = Removed;
            evt?.Invoke(wrapper);
        }

        private bool TryRemove(int actorId, out Actor<TComponent1, TComponent2> wrapper)
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

        public void Dispose()
        {
            _context.Added -= OnActorAdded;
            _context.ComponentAdded -= OnActorComponentAdded;
            _context.Removed -= OnActorRemoved;

            _lock.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}