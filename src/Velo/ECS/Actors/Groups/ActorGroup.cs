using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Velo.Collections;
using Velo.ECS.Actors.Context;
using Velo.Threading;

namespace Velo.ECS.Actors.Groups
{
    internal sealed class ActorGroup<TActor> : IActorGroup<TActor>, IDisposable
        where TActor : Actor
    {
        public event Action<TActor>? Added;

        public event Action<TActor>? Removed;

        public int Length => _actors.Count;

        private readonly List<TActor> _actors;
        private readonly IActorContext _context;
        private readonly ReaderWriterLockSlim _lock;

        public ActorGroup(IActorContext context)
        {
            _actors = new List<TActor>();
            _lock = new ReaderWriterLockSlim();

            context.Added += OnActorAdded;
            context.Removed += OnActorRemoved;

            _context = context;
        }

        public bool Contains(int actorId) => TryGet(actorId, out _);

        public IEnumerator<TActor> GetEnumerator()
        {
            return new ReadLockEnumerator<TActor>(_actors, _lock);
        }

        public bool TryGet(int actorId, out TActor actor)
        {
            using (ReadLock.Enter(_lock))
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var exists in _actors)
                {
                    if (exists.Id != actorId) continue;

                    actor = exists;
                    return true;
                }
            }

            actor = default!;
            return false;
        }

        public IEnumerable<TActor> Where<TArg>(Func<TActor, TArg, bool> filter, TArg arg)
        {
            return new ReadLockWhereEnumerator<TActor, TArg>(_actors, filter, arg, _lock);
        }

        private void OnActorAdded(Actor actor)
        {
            if (!(actor is TActor found)) return;

            _actors.Add(found);

            var evt = Added;
            evt?.Invoke(found);
        }

        private void OnActorRemoved(Actor actor)
        {
            if (!(actor is TActor found)) return;
            if (!_actors.Remove(found)) return;

            var evt = Removed;
            evt?.Invoke(found);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Dispose()
        {
            _context.Added += OnActorAdded;
            _context.Removed += OnActorRemoved;

            _lock.Dispose();
        }
    }
}