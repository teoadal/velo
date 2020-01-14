using System;
using System.Diagnostics;
using System.Threading;

namespace Velo.Pools
{
    public sealed class Pool<T> : IPool<T>
        where T : class
    {
        private readonly T[] _buffer;

        private int _index;
        private SpinLock _lock;

        public Pool(int capacity, Func<T> builder)
        {
            var buffer = new T[capacity];
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = builder();
            }

            _buffer = buffer;
            _index = 0;
            _lock = new SpinLock(Debugger.IsAttached);
        }

        public Pool(T[] buffer)
        {
            _buffer = buffer;
            _index = 0;
            _lock = new SpinLock(Debugger.IsAttached);
        }

        public T Get()
        {
            while (true)
            {
                if (!TryGet(out var element)) continue;
                return element;
            }
        }

        public bool Return(T element)
        {
            var lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);

                if ((uint) _index < (uint) _buffer.Length)
                {
                    _buffer[--_index] = element;
                    return true;
                }
            }
            finally
            {
                if (lockTaken) _lock.Exit(false);
            }

            return false;
        }

        public bool TryGet(out T element)
        {
            var lockTaken = false;
            try
            {
                _lock.Enter(ref lockTaken);

                var buffer = _buffer;
                if ((uint) _index < (uint) buffer.Length)
                {
                    element = buffer[_index];
                    buffer[_index++] = null;
                    return true;
                }
            }
            finally
            {
                if (lockTaken) _lock.Exit(false);
            }

            element = default;
            return false;
        }
    }
}