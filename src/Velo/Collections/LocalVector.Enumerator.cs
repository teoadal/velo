using System.Runtime.CompilerServices;

namespace Velo.Collections
{
    public ref partial struct LocalVector<T>
    {
        public ref struct Enumerator
        {
            public readonly T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _vector.Get(_position);
            }

            public readonly int Length;
            
            private int _position;
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            private LocalVector<T> _vector;

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
                _position++;
                return _position < Length;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _position = -1;
            }
        }
    }
}