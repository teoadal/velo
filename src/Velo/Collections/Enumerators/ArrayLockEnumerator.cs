using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Velo.Collections.Enumerators
{
    internal struct ArrayLockEnumerator<T> : IEnumerator<T>, IEnumerable<T>
    {
        public T Current { get; private set; }

        private T[] _array;
        private ReaderWriterLockSlim _lock;
        private int _position;

        internal ArrayLockEnumerator(T[] array, ReaderWriterLockSlim lockObject)
        {
            _array = array;

            _lock = lockObject;
            _lock.EnterReadLock();

            Current = default!;

            _position = -1;
        }

        public IEnumerator<T> GetEnumerator() => this;

        public bool MoveNext()
        {
            _position++;

            if (_position == _array.Length) return false;

            Current = _array[_position];

            return true;
        }

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => Current!;
        IEnumerator IEnumerable.GetEnumerator() => this;

        public void Dispose()
        {
            _lock.ExitReadLock();

            Current = default!;

            _array = null!;
            _lock = null!;
        }
    }
}