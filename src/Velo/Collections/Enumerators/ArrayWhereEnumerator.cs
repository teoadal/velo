using System;
using System.Collections;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Collections.Enumerators
{
    internal struct ArrayWhereEnumerator<T, TArg> : IEnumerator<T>, IEnumerable<T>
    {
        public T Current { get; private set; }

        private TArg _arg;
        private T[] _array;
        private Func<T, TArg, bool> _filter;

        private bool _disposed;
        private int _position;

        public ArrayWhereEnumerator(T[] array, Func<T, TArg, bool> filter, TArg arg)
        {
            _array = array;
            _arg = arg;
            _disposed = false;
            _filter = filter;
            _position = 0;

            Current = default!;
        }

        public bool MoveNext()
        {
            if (_disposed) throw Error.Disposed(GetType().Name);
            
            for (; _position < _array.Length; _position++)
            {
                var current = _array[_position];

                if (!_filter(current, _arg)) continue;

                _position++;
                Current = current;
                return true;
            }

            return false;
        }

        public readonly IEnumerator<T> GetEnumerator() => this;

        public void Reset()
        {
            _position = 0;
        }

        public void Dispose()
        {
            if (_disposed) return;
            
            _arg = default!;
            _array = null!;
            _filter = null!;

            Current = default!;

            _disposed = true;
        }

        object IEnumerator.Current => Current!;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}