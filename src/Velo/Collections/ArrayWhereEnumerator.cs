using System;
using System.Collections;
using System.Collections.Generic;

namespace Velo.Collections
{
    internal struct ArrayWhereEnumerator<T, TArg> : IEnumerator<T>, IEnumerable<T>
    {
        public T Current { get; private set; }

        private TArg _arg;
        private T[] _array;
        private Func<T, TArg, bool> _filter;
        private int _position;

        public ArrayWhereEnumerator(T[] array, Func<T, TArg, bool> filter, TArg arg)
        {
            _array = array;
            _filter = filter;
            _arg = arg;
            _position = 0;

            Current = default!;
        }

        public bool MoveNext()
        {
            for (var i = _position; i < _array.Length; i++)
            {
                var current = _array[i];

                if (!_filter(current, _arg)) continue;

                _position++;
                Current = current;
                return true;
            }

            return false;
        }

        public IEnumerator<T> GetEnumerator() => this;

        public void Reset()
        {
            _position = -1;
        }

        public void Dispose()
        {
            _arg = default!;
            _array = null!;
            _filter = null!;
            _position = -1;

            Current = default!;
        }

        object IEnumerator.Current => Current!;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}