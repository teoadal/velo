using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Velo.Utils;

namespace Velo.Collections.Enumerators
{
    internal struct ArrayLockEnumerator<T> : IEnumerator<T>, IEnumerable<T>
    {
        public T Current { get; private set; }

        private T[] _array;
        private ReaderWriterLockSlim _lock;
        private int _position;

        private bool _disposed;

        internal ArrayLockEnumerator(T[] array, ReaderWriterLockSlim lockObject)
        {
            _array = array;
            _disposed = false;

            _lock = lockObject;
            _lock.EnterReadLock();

            _position = 0;

            Current = default!;
        }

        public readonly ArrayLockEnumerator<T> GetEnumerator() => this;

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

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => Current!;
        IEnumerator IEnumerable.GetEnumerator() => this;
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;

        public void Dispose()
        {
            if (_disposed) return;

            _lock.ExitReadLock();

            Current = default!;

            _array = null!;
            _lock = null!;

            _disposed = true;
        }
    }
}