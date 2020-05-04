using System.Diagnostics;

namespace Velo.Collections.Local
{
    internal readonly ref struct LocalListDebugVisualizer<T>
    {
        private readonly LocalList<T> _list;

        public LocalListDebugVisualizer(LocalList<T> list)
        {
            _list = list;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _list.ToArray();
    }
}