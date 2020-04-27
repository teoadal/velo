using System;
using System.Collections.Generic;
using System.Threading;
using Velo.Utils;

namespace Velo.Collections
{
    internal interface IVector<TKey, TValue>
    {
        void ClearSafe();

        TValue GetOrAdd(TKey key, Func<TKey, TValue> factory);

        TValue GetOrAdd<TArg>(TKey key, Func<TKey, TArg, TValue> factory, TArg arg);

        void TryAdd(TKey key, TValue value);
    }

    /// <summary>
    /// Avoid closure for GetOrAdd and basic dangerous concurrent (only add in lock).
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal class DangerousVector<TKey, TValue> : Dictionary<TKey, TValue>, IVector<TKey, TValue>
    {
        private readonly object _lock;

        public DangerousVector() : this(10)
        {
        }

        public DangerousVector(IDictionary<TKey, TValue> source) 
            : base(source)
        {
            _lock = new object();
        }
        
        public DangerousVector(int capacity)
            : base(capacity < 10 ? throw Error.InvalidOperation("Capacity less 10") : capacity)
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

        public void TryAdd(TKey key, TValue value)
        {
            var lockTaken = false;
            Monitor.Enter(_lock, ref lockTaken);

            this[key] = value;

            if (lockTaken) Monitor.Exit(_lock);
        }
    }
}