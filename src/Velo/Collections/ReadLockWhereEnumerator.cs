using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Velo.Collections
{
    internal struct ReadLockWhereEnumerator<T, TArg> : IEnumerator<T>, IEnumerable<T>
    {
        public T Current { get; private set; }

        private readonly TArg _arg;
        private readonly Func<T, TArg, bool> _filter;
        private List<T>.Enumerator _enumerator;
        private readonly ReaderWriterLockSlim _lockObject;

        public ReadLockWhereEnumerator(List<T> list, Func<T, TArg, bool> filter, TArg arg,
            ReaderWriterLockSlim lockObject)
        {
            _filter = filter;
            _arg = arg;
            _lockObject = lockObject;

            _lockObject.EnterReadLock();
            _enumerator = list.GetEnumerator(); // after enter read lock

            Current = default;
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

        object IEnumerator.Current => Current;

        IEnumerator IEnumerable.GetEnumerator() => this;

        public void Dispose()
        {
            _lockObject.ExitReadLock();
            _enumerator.Dispose();
        }
    }
}