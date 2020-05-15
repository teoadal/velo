using System.Threading;

namespace Velo.Threading
{
    public ref struct Lock
    {
        private readonly object _lockObject;
        private bool _lockTaken;

        private Lock(object lockObject)
        {
            _lockObject = lockObject;
            _lockTaken = false;
        }

        public static Lock Enter(object lockObject)
        {
            var locker = new Lock(lockObject);
            Monitor.Enter(lockObject, ref locker._lockTaken);
            return locker;
        }

        public readonly void Dispose()
        {
            if (_lockTaken) Monitor.Exit(_lockObject);
        }
    }

    public readonly ref struct ReadLock
    {
        private readonly ReaderWriterLockSlim _lockObject;

        private ReadLock(ReaderWriterLockSlim lockObject)
        {
            _lockObject = lockObject;
        }

        public static ReadLock Enter(ReaderWriterLockSlim lockObject)
        {
            var locker = new ReadLock(lockObject);
            lockObject.EnterReadLock();
            return locker;
        }

        public void Dispose()
        {
            _lockObject.ExitReadLock();
        }
    }

    public readonly ref struct WriteLock
    {
        private readonly ReaderWriterLockSlim _lockObject;

        private WriteLock(ReaderWriterLockSlim lockObject)
        {
            _lockObject = lockObject;
        }

        public static WriteLock Enter(ReaderWriterLockSlim lockObject)
        {
            var locker = new WriteLock(lockObject);
            lockObject.EnterWriteLock();
            return locker;
        }

        public void Dispose()
        {
            _lockObject.ExitWriteLock();
        }
    }
}