using System.Diagnostics;
using System.Linq;
using Velo.ECS.Components;

namespace Velo.ECS.Assets.Filters
{
    internal sealed class DebuggerVisualizer<TComponent>
        where TComponent : IComponent
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Asset<TComponent>[] Items => _filter.ToArray();

        private readonly AssetFilter<TComponent> _filter;

        public DebuggerVisualizer(AssetFilter<TComponent> filter)
        {
            _filter = filter;
        }
    }
}