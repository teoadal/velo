using System.Diagnostics;
using System.Linq;
using Velo.ECS.Components;

namespace Velo.ECS.Actors.Filters
{
    internal sealed class DebuggerVisualizer<TComponent>
        where TComponent : IComponent
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Actor<TComponent>[] Items => _filter.ToArray();

        private readonly ActorFilter<TComponent> _filter;

        public DebuggerVisualizer(ActorFilter<TComponent> filter)
        {
            _filter = filter;
        }
    }
}