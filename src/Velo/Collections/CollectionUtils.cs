using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Collections
{
    internal static class CollectionUtils
    {
        public static void Add<T>(ref T[] array, T element)
        {
            var arrayLength = array.Length;
            Array.Resize(ref array, arrayLength + 1);
            array[arrayLength] = element;
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

        public static void EnsureCapacity<T>(ref T[] array, int capacity)
        {
            if (array.Length < capacity)
            {
                Array.Resize(ref array, capacity);
            }
        }

        public static void EnsureUnique<T>(T[] array, Action<T> notUniqueHandler)
        {
            var hashSet = new HashSet<T>();
            foreach (var element in array)
            {
                if (!hashSet.Add(element))
                {
                    notUniqueHandler(element);
                }
            }
        }

        public static void Insert<T>(ref T[] array, int index, T element)
        {
            var arrayLength = array.Length;
            if ((uint) index < (uint) arrayLength)
            {
                array[index] = element;
            }
            else
            {
                var newLength = arrayLength == 0 ? 2 : arrayLength * 2;
                Array.Resize(ref array, newLength);
                array[index] = element;
            }
        }

        public static void RemoveAt<T>(ref T[] array, int index)
        {
            var length = array.Length - 1;

            if (index < length)
            {
                Array.Copy(array, index + 1, array, index, length - index);
            }

            array[length] = default!;
        }
    }
}