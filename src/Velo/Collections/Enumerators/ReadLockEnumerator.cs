using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Velo.Collections.Enumerators
{
    internal struct ReadLockEnumerator<T> : IEnumerator<T>
    {
        public T Current { get; private set; }

        private List<T>.Enumerator _enumerator;
        private ReaderWriterLockSlim _lock;

        internal ReadLockEnumerator(List<T> list, ReaderWriterLockSlim lockObject)
        {
            _lock = lockObject;
            _lock.EnterReadLock();

            _enumerator = list.GetEnumerator(); // after enter read lock

            Current = default!;
        }

        public bool MoveNext()
        {
            if (!_enumerator.MoveNext()) return false;

            Current = _enumerator.Current;

            return true;
        }

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => Current!;

        public void Dispose()
        {
            _lock.ExitReadLock();
            _enumerator.Dispose();

            Current = default!;

            _enumerator = default;
            _lock = null!;
        }
    }

    internal struct ReadLockEnumerator<TKey, TValue> : IEnumerator<TValue>
    {
        public TValue Current { get; private set; }

        private Dictionary<TKey, TValue>.ValueCollection.Enumerator _enumerator;
        private readonly ReaderWriterLockSlim _lock;

        internal ReadLockEnumerator(
            Dictionary<TKey, TValue>.ValueCollection valueCollection,
            ReaderWriterLockSlim lockObject)
        {
            _lock = lockObject;
            _lock.EnterReadLock();

            _enumerator = valueCollection.GetEnumerator(); // after enter read lock

            Current = default!;
        }

        public bool MoveNext()
        {
            if (!_enumerator.MoveNext()) return false;

            Current = _enumerator.Current;

            return true;
        }

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => Current!;

        public void Dispose()
        {
            _lock.ExitReadLock();
            _enumerator.Dispose();
        }
    }
}