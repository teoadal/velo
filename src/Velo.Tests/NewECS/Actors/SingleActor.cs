using System;
using System.Threading;
using Velo.Tests.NewECS.Actors.Context;
using Velo.Utils;

namespace Velo.Tests.NewECS.Actors
{
    public sealed class SingleActor<TActor> : IDisposable
        where TActor : Actor
    {
        public bool Exists => Volatile.Read(ref _instance) != null;

        private TActor _instance;
        private readonly IActorContext _context;

        internal SingleActor(IActorContext context)
        {
            foreach (var actor in context)
            {
                if (!(actor is TActor single)) continue;

                _instance = single;
                break;
            }

            context.Added += OnAdded;
            context.Removed += OnRemoved;

            _context = context;
        }

        public TActor GetInstance()
        {
            var instance = Volatile.Read(ref _instance);

            if (instance == null)
            {
                throw Error.NotFound($"Single actor {ReflectionUtils.GetName<TActor>()} not added in context");
            }

            return instance;
        }

        private void OnAdded(Actor actor)
        {
            if (!(actor is TActor single)) return;

            Volatile.Write(ref _instance, single);
        }

        private void OnRemoved(Actor actor)
        {
            if (actor is TActor)
            {
                Volatile.Write(ref _instance, null);
            }
        }

        public static implicit operator TActor(SingleActor<TActor> single)
        {
            return single.GetInstance();
        }

        public bool TryGetInstance(out TActor actor)
        {
            actor = Volatile.Read(ref _instance);
            return actor != null;
        }

        public void Dispose()
        {
            _context.Added -= OnAdded;
            _context.Removed -= OnRemoved;

            _instance = null;
        }
    }
}