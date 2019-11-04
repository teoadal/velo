using System.Collections.Generic;

namespace Velo.Collections
{
    public ref partial struct LocalVector<T>
    {
        public ref struct Enumerator
        {
            public T Current => _current!;

            public readonly int Length;

            private readonly T _element0;
            private readonly T _element1;
            private readonly T _element2;
            private readonly T _element3;
            private readonly T _element4;
            private readonly T _element5;
            private readonly List<T> _list;
            
            private T _current;
            private int _position;

            internal Enumerator(T element0, T element1, T element2, T element3, T element4, T element5, 
                List<T> list, int length)
            {
                Length = length;

                _element0 = element0;
                _element1 = element1;
                _element2 = element2;
                _element3 = element3;
                _element4 = element4;
                _element5 = element5;
                _list = list;

                _current = default;
                _position = 0;
            }

            public bool MoveNext()
            {
                if (_position == Length) return false;

                switch (_position)
                {
                    case 0:
                        _current = _element0;
                        break;
                    case 1:
                        _current = _element1;
                        break;
                    case 2:
                        _current = _element2;
                        break;
                    case 3:
                        _current = _element3;
                        break;
                    case 4:
                        _current = _element4;
                        break;
                    case 5:
                        _current = _element5;
                        break;
                    default:
                        _current = _list[_position - Capacity];
                        break;
                }

                _position++;

                return true;
            }

            public void Reset()
            {
                _current = default;
                _position = 0;
            }
        }
    }
}