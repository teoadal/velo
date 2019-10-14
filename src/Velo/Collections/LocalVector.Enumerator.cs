using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Velo.Collections
{
    public partial struct LocalVector<T>
    {
        public struct Enumerator : IEnumerator<T>
        {
            public T Current { get; private set; }

            private T _element0;
            private T _element1;
            private T _element2;
            private T _element3;
            private T _element4;
            private Sequence<T> _sequence;
            private readonly int _length;

            private int _position;

            internal Enumerator(T element0, T element1, T element2, T element3, T element4, Sequence<T> sequence,
                int length)
            {
                _element0 = element0;
                _element1 = element1;
                _element2 = element2;
                _element3 = element3;
                _element4 = element4;
                _sequence = sequence;
                _length = length;

                _position = 0;

                Current = default;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (_position == _length) return false;

                switch (_position)
                {
                    case 0:
                        Current = _element0;
                        break;
                    case 1:
                        Current = _element1;
                        break;
                    case 2:
                        Current = _element2;
                        break;
                    case 3:
                        Current = _element3;
                        break;
                    case 4:
                        Current = _element4;
                        break;
                    default:
                        Current = _sequence[_position - 5];
                        break;
                }

                _position++;

                return true;
            }

            void IEnumerator.Reset()
            {
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _element0 = default;
                _element1 = default;
                _element2 = default;
                _element3 = default;
                _element4 = default;

                _sequence = null;
            }
        }
    }
}