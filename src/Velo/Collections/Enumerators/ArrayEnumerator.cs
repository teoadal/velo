using System.Collections;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Collections.Enumerators
{
    public struct ArrayEnumerator<T> : IEnumerator<T>, IEnumerable<T>
    {
        public T Current { get; private set; }

        private T[] _array;
        private int _position;

        private bool _disposed;

        internal ArrayEnumerator(T[] array)
        {
            _array = array;
            _disposed = false;
            _position = 0;

            Current = default!;
        }

        public bool MoveNext()
        {
            if (_disposed) throw Error.Disposed(GetType().Name);

            // ReSharper disable once InvertIf
            if ((uint) _position < (uint) _array.Length)
            {
                Current = _array[_position++];
                return true;
            }

            return false;
        }

        public readonly IEnumerator<T> GetEnumerator() => this;

        public void Reset()
        {
            if (_disposed) throw Error.Disposed(GetType().Name);
            _position = 0;
        }

        public void Dispose()
        {
            if (_disposed) return;

            Current = default!;

            _array = null!;
            _position = -1;

            _disposed = true;
        }

        object IEnumerator.Current => Current!;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}