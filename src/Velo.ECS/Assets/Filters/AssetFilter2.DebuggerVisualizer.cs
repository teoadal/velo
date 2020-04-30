using System.Diagnostics;
using System.Linq;
using Velo.ECS.Components;

namespace Velo.ECS.Assets.Filters
{
    internal sealed class DebuggerVisualizer<TComponent1, TComponent2>
        where TComponent1: IComponent where TComponent2: IComponent
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Asset<TComponent1, TComponent2>[] Items => _filter.ToArray();

        private readonly AssetFilter<TComponent1, TComponent2> _filter;

        public DebuggerVisualizer(AssetFilter<TComponent1, TComponent2> filter)
        {
            _filter = filter;
        }
    }
}