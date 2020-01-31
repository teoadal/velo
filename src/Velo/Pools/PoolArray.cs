using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Pools
{
    internal sealed class PoolArray<T> : IArrayPool<T>
    {
        private readonly ConcurrentDictionary<int, Pool<T[]>> _buckets;
        private readonly int _capacity;
        private readonly EmptyPool<T[]> _default;
        private readonly Func<int, Pool<T[]>> _poolBuilder;

        public PoolArray(int capacity = 10)
        {
            _buckets = new ConcurrentDictionary<int, Pool<T[]>>(EqualityComparer<int>.Default);
            _capacity = capacity;
            _default = new EmptyPool<T[]>(Array.Empty<T>());
            _poolBuilder = BuildBucket;
        }

        public T[] Get(int length)
        {
            var bucket = GetPool(length);
            return bucket.Get();
        }

        public IPool<T[]> GetPool(int length)
        {
            if (length == 0) return _default;
            if (length < 0) throw Error.InvalidOperation("Array length must greater or equal 0");

            return _buckets.GetOrAdd(length, _poolBuilder);
        }

        public void Return(T[] array, bool clearArray = false)
        {
            if (clearArray) Array.Clear(array, 0, array.Length);

            var pool = GetPool(array.Length);
            pool.Return(array);
        }

        public bool TryGet(int length, out T[] array)
        {
            var bucket = GetPool(length);
            return bucket.TryGet(out array);
        }

        private Pool<T[]> BuildBucket(int arrayLength)
        {
            var buffer = new T[_capacity][];
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = new T[arrayLength];
            }

            return new Pool<T[]>(buffer);
        }
    }
}