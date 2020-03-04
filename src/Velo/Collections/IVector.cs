using System;
using System.Collections.Generic;
using System.Threading;

namespace Velo.Collections
{
    internal interface IVector<TKey, TValue>
    {
        void ClearSafe();

        TValue GetOrAdd(TKey key, Func<TKey, TValue> factory);

        TValue GetOrAdd<TArg>(TKey key, Func<TKey, TArg, TValue> factory, TArg arg);
    }

    /// <summary>
    /// Avoid closure for GetOrAdd and basic dangerous concurrent (only add in lock).
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class DangerousVector<TKey, TValue> : Dictionary<TKey, TValue>, IVector<TKey, TValue>
    {
        private readonly object _lock;

        public DangerousVector() : base(10)
        {
            _lock = new object();
        }

        public void ClearSafe()
        {
            var lockTaken = false;
            Monitor.Enter(_lock, ref lockTaken);

            Clear();

            if (lockTaken) Monitor.Exit(_lock);
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> factory)
        {
            if (TryGetValue(key, out var exists)) return exists;

            var value = factory(key);

            var lockTaken = false;
            Monitor.Enter(_lock, ref lockTaken);

            this[key] = value;

            if (lockTaken) Monitor.Exit(_lock);

            return value;
        }

        public TValue GetOrAdd<TArg>(TKey key, Func<TKey, TArg, TValue> factory, TArg arg)
        {
            if (TryGetValue(key, out var exists)) return exists;

            var value = factory(key, arg);

            var lockTaken = false;
            Monitor.Enter(_lock, ref lockTaken);

            this[key] = value;

            if (lockTaken) Monitor.Exit(_lock);

            return value;
        }
    }
}