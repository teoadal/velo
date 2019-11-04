using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Velo.Utils
{
    internal static class CollectionUtils
    {
        public static void DisposeValuesIfDisposable<TKey, TValue>(ConcurrentDictionary<TKey, TValue> dictionary)
            where TValue: class
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
            where TValue: class
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
            where T: class
        {
            for (var i = 0; i < array.Length; i++)
            {
                var value = array[i];
                if (ReflectionUtils.IsDisposable(value, out var disposable))
                {
                    disposable.Dispose();
                }
            }
        }
    }
}