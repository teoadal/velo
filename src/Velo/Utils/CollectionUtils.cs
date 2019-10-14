using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Velo.Utils
{
    internal static class CollectionUtils
    {
        public static void DisposeValuesIfDisposable<TKey, TValue>(ConcurrentDictionary<TKey, TValue> dictionary)
            where TValue: class
        {
            foreach (var value in dictionary.Values)
            {
                if (value != null && value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
        
        public static void DisposeValuesIfDisposable<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
            where TValue: class
        {
            foreach (var value in dictionary.Values)
            {
                if (value != null && value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
        
        public static void DisposeValuesIfDisposable<T>(T[] array)
            where T: class
        {
            for (var i = 0; i < array.Length; i++)
            {
                var value = array[i];
                if (value != null && value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        public static void Fill<T>(T[] array, T value)
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveByIndex<T>(T[] array, int index, int length)
        {
            length--;
            Array.Copy(array, index + 1, array, index, length - index);
            array[length] = default;
        }
    }
}