using System.Collections;
using System.Collections.Generic;

namespace Velo.Collections
{
    public sealed partial class Sequence<T>
    {
        public struct Enumerator : IEnumerator<T>
        {
            public T Current { get; private set; }

            private T[] _array;
            private readonly uint _length;

            private int _index;

            internal Enumerator(T[] array, int length)
            {
                _array = array;
                _length = (uint) length;
                _index = 0;

                Current = default;
            }

            public bool MoveNext()
            {
                if ((uint) _index >= _length) return false;

                Current = _array[_index++];
                return true;
            }

            object IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
            }

            public void Dispose()
            {
                _array = null;
                Current = default;
            }
        }
    }
}