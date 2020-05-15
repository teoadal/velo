using System.Linq;
using Velo.ECS.Actors.Filters;
using Velo.ECS.Actors.Groups;

namespace Velo.ECS.Actors.Context
{
    internal sealed partial class ActorContext
    {
        private sealed class ActorContextDebugVisualizer
        {
            // ReSharper disable UnusedMember.Local

            public Actor[] Actors => _context._actors.Values.ToArray();

            public IActorFilter[] Filters => _context._filters.Values.ToArray();

            public IActorGroup[] Groups => _context._groups.Values.ToArray();

            public object[] Singles => _context._singleActors.Values.ToArray();

            // ReSharper restore UnusedMember.Local

            private readonly ActorContext _context;

            public ActorContextDebugVisualizer(ActorContext context)
            {
                _context = context;
            }
        }
    }
}