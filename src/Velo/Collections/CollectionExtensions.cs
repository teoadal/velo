using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Velo.Collections
{
    public static class CollectionExtensions
    {
        public static bool Contains<T, TArg>(this T[] array, Func<T, TArg, bool> predicate, TArg arg)
        {
            foreach (var element in array)
            {
                if (predicate(element, arg)) return true;
            }

            return false;
        }

        public static IEnumerable<T> Do<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var element in collection)
            {
                action(element);
                yield return element;
            }
        }

        public static void Foreach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var element in collection)
            {
                action(element);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool NullOrEmpty<T>(this T[] array)
        {
            return array == null || array.Length == 0;
        }
    }
}