using System.Runtime.CompilerServices;

namespace Velo.Collections
{
    public ref partial struct LocalVector<T>
    {
        public ref struct Enumerator
        {
            public T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _vector.Get(_position);
            }

            public readonly int Length;
            
            private readonly LocalVector<T> _vector;
            
            private int _position;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(LocalVector<T> vector)
            {
                Length = vector.Length;
                
                _position = -1;
                _vector = vector;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                var index = _position + 1;
                if (index >= Length) return false;
                
                _position = index;
                return true;
            }

            public void Reset()
            {
                _position = 0;
            }
        }
    }
}