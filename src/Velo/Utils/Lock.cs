using System.Threading;

namespace Velo.Utils
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
        
        public void Dispose()
        {
            if (_lockTaken) Monitor.Exit(_lockObject);
        }
    }
}