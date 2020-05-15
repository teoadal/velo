using System.Diagnostics;
using System.Linq;

namespace Velo.ECS.Actors.Groups
{
    internal sealed class DebuggerVisualizer<TActor>
        where TActor : Actor
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public TActor[] Items => _group.ToArray();

        private readonly ActorGroup<TActor> _group;

        public DebuggerVisualizer(ActorGroup<TActor> actorGroup)
        {
            _group = actorGroup;
        }
    }
}