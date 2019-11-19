using System.Diagnostics;

namespace Velo.Collections
{
    internal readonly ref struct LocalVectorDebugVisualizer<T>
    {
        private readonly LocalVector<T> _vector;

        public LocalVectorDebugVisualizer(LocalVector<T> vector)
        {
            _vector = vector;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _vector.ToArray();
    }
}