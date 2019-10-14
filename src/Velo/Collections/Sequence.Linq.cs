using System;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.Collections
{
    public sealed partial class Sequence<T>
    {
        public bool Any(Predicate<T> predicate)
        {
            return FindIndex(predicate) != -1;
        }

        public bool Any<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            return FindIndex(predicate, arg) != -1;
        }

        public int Count(Predicate<T> predicate)
        {
            var array = _array;
            var length = _length;

            var counter = 0;
            for (var i = 0; i < array.Length; i++)
            {
                if (i == length) break;
                if (predicate(array[i])) counter++;
            }

            return counter;
        }

        public int Count<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            var array = _array;
            var length = _length;

            var counter = 0;
            for (var i = 0; i < array.Length; i++)
            {
                if (i == length) break;
                if (predicate(array[i], arg)) counter++;
            }

            return counter;
        }

        public T First(Predicate<T> predicate)
        {
            var index = FindIndex(predicate);
            return index == -1
                ? throw Error.NotFound()
                : _array[index];
        }

        public T First<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            var index = FindIndex(predicate, arg);
            return index == -1
                ? throw Error.NotFound()
                : _array[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T FirstOrDefault(Predicate<T> predicate)
        {
            var index = FindIndex(predicate);
            return index == -1
                ? default
                : _array[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T FirstOrDefault<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            var index = FindIndex(predicate, arg);
            return index == -1
                ? default
                : _array[index];
        }

        public bool Remove(Predicate<T> predicate)
        {
            var index = FindIndex(predicate);
            if (index == -1) return false;

            RemoveAt(index);
            return true;
        }

        public bool Remove<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            var index = FindIndex(predicate, arg);
            if (index == -1) return false;

            RemoveAt(index);
            return true;
        }

        public SelectEnumerator<TValue> Select<TValue>(Func<T, TValue> selector)
        {
            return new SelectEnumerator<TValue>(_array, _length, selector);
        }

        public WhereEnumerator Where(Predicate<T> predicate)
        {
            return new WhereEnumerator(_array, _length, predicate);
        }

        public WhereEnumerator<TArg> Where<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            return new WhereEnumerator<TArg>(_array, _length, predicate, arg);
        }
    }
}