using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Velo.Utils
{
    internal static class CollectionUtils
    {
        public static void Cut<T>(ref T[] array, int index)
        {
            var length = array.Length - 1;

            if (index < length)
            {
                Array.Copy(array, index + 1, array, index, length - index);
            }

            array[length] = default;
        }

        public static void DisposeValuesIfDisposable<TKey, TValue>(ConcurrentDictionary<TKey, TValue> dictionary)
            where TValue : class
        {
            foreach (var value in dictionary.Values)
            {
                if (ReflectionUtils.IsDisposable(value, out var disposable))
                {
                    disposable.Dispose();
                }
            }
        }

        public static void DisposeValuesIfDisposable<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
            where TValue : class
        {
            foreach (var value in dictionary.Values)
            {
                if (ReflectionUtils.IsDisposable(value, out var disposable))
                {
                    disposable.Dispose();
                }
            }
        }

        public static void DisposeValuesIfDisposable<T>(T[] array)
            where T : class
        {
            foreach (var value in array)
            {
                if (ReflectionUtils.IsDisposable(value, out var disposable))
                {
                    disposable.Dispose();
                }
            }
        }
    }
}