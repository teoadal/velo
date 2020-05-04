using System.Runtime.CompilerServices;

namespace Velo.Collections.Local
{
    public ref partial struct LocalList<T>
    {
        public ref struct Enumerator
        {
            public readonly T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => _list.Get(_position);
            }

            public readonly int Length;
            
            private int _position;
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            private LocalList<T> _list;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Enumerator(in LocalList<T> list)
            {
                Length = list.Length;

                _position = -1;
                _list = list;
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