using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Velo.Collections.Enumerators
{
    internal struct ReadLockWhereEnumerator<T, TArg> : IEnumerator<T>, IEnumerable<T>
    {
        public T Current { get; private set; }

        private TArg _arg;
        private Func<T, TArg, bool> _filter;
        private List<T>.Enumerator _enumerator;
        private ReaderWriterLockSlim _lockObject;

        public ReadLockWhereEnumerator(
            List<T> list,
            Func<T, TArg, bool> filter,
            TArg arg,
            ReaderWriterLockSlim lockObject)
        {
            _filter = filter;
            _arg = arg;
            _lockObject = lockObject;

            _lockObject.EnterReadLock();
            _enumerator = list.GetEnumerator(); // after enter read lock

            Current = default!;
        }

        public IEnumerator<T> GetEnumerator() => this;

        public bool MoveNext()
        {
            while (_enumerator.MoveNext())
            {
                var current = _enumerator.Current;

                if (!_filter(current, _arg)) continue;

                Current = current;
                return true;
            }

            return false;
        }

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => Current!;

        IEnumerator IEnumerable.GetEnumerator() => this;

        public void Dispose()
        {
            _lockObject.ExitReadLock();
            _enumerator.Dispose();

            Current = default!;
            
            _arg = default!;
            _filter = null!;
            _enumerator = default;
            _lockObject = null!;
        }
    }

    internal struct ReadLockWhereEnumerator<TKey, TValue, TArg> : IEnumerator<TValue>, IEnumerable<TValue>
    {
        public TValue Current { get; private set; }

        private TArg _arg;
        private Func<TValue, TArg, bool> _filter;
        private Dictionary<TKey, TValue>.ValueCollection.Enumerator _enumerator;
        private ReaderWriterLockSlim _lockObject;

        public ReadLockWhereEnumerator(
            Dictionary<TKey, TValue>.ValueCollection valueCollection,
            Func<TValue, TArg, bool> filter,
            TArg arg,
            ReaderWriterLockSlim lockObject)
        {
            _filter = filter;
            _arg = arg;
            _lockObject = lockObject;

            _lockObject.EnterReadLock();
            _enumerator = valueCollection.GetEnumerator(); // after enter read lock

            Current = default!;
        }

        public IEnumerator<TValue> GetEnumerator() => this;

        public bool MoveNext()
        {
            while (_enumerator.MoveNext())
            {
                var current = _enumerator.Current;

                if (!_filter(current, _arg)) continue;

                Current = current;
                return true;
            }

            return false;
        }

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => Current!;

        IEnumerator IEnumerable.GetEnumerator() => this;

        public void Dispose()
        {
            _lockObject.ExitReadLock();
            _enumerator.Dispose();

            _arg = default!;
            _filter = null!;
            _enumerator = default;
            _lockObject = null!;
        }
    }
}