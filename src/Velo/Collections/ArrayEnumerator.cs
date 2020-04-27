using System.Collections;
using System.Collections.Generic;

namespace Velo.Collections
{
    public struct ArrayEnumerator<T> : IEnumerator<T>, IEnumerable<T>
    {
        public T Current { get; private set; }

        private T[] _array;
        private int _position;

        public ArrayEnumerator(T[] array)
        {
            _array = array;
            _position = -1;

            Current = default!;
        }

        public bool MoveNext()
        {
            _position++;

            if (_position == _array.Length) return false;

            Current = _array[_position];

            return true;
        }

        public IEnumerator<T> GetEnumerator() => this;

        public void Reset()
        {
            _position = -1;
        }

        public void Dispose()
        {
            Current = default!;

            _array = null!;
            _position = -1;
        }

        object IEnumerator.Current => Current!;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}