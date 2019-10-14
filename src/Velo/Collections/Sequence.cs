using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.Collections
{
    public sealed partial class Sequence<T> : IEnumerable<T>
    {
        public int Length => _length;

        private T[] _array;
        private int _length;
        private readonly bool _referenceValues;
        private readonly Func<int, int> _resizeRule;

        public Sequence(int capacity = 4, Func<int, int> resizeRule = null)
        {
            _array = capacity == 0 ? Array.Empty<T>() : new T[capacity];
            _resizeRule = resizeRule ?? SequenceResizeRule.Default;
            _referenceValues = Typeof<T>.IsReferenceType;
        }

        public Sequence(ICollection<T> collection, Func<int, int> resizeRule = null)
            : this(collection.Count, resizeRule)
        {
            _length = collection.Count;

            collection.CopyTo(_array, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T element)
        {
            var array = _array;
            var length = _length;

            if ((uint) length < (uint) array.Length)
            {
                _length = length + 1;
                array[length] = element;
            }
            else
            {
                AddWithResize(element);
            }
        }

        public void Clear()
        {
            if (_referenceValues)
            {
                Array.Clear(_array, 0, _length);
            }
            
            WeakClear();
        }

        public bool Contains(T element)
        {
            var length = _length;
            return length != 0 && Array.IndexOf(_array, element, 0, length) != -1;
        }

        #region GetEnumerator

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator GetEnumerator() => new Enumerator(_array, _length);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _length == 0 ? EmptyEnumerator<T>.Instance : GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _length == 0 ? EmptyEnumerator<T>.Instance : GetEnumerator();
        }

        #endregion

        public T[] GetUnderlyingArray(out int length)
        {
            length = _length;
            return _array;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindIndex(Predicate<T> predicate)
        {
            return Array.FindIndex(_array, 0, _length, predicate);
        }
        
        public int FindIndex<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            var array = _array;
            var length = _length;
            
            for (var i = 0; i < array.Length; i++)
            {
                if (i == length) break;
                if (predicate(array[i], arg)) return i;
            }

            return -1;
        }
        
        public bool Remove(T element)
        {
            var index = Array.IndexOf(_array, element, 0, _length);
            if (index == -1) return false;

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            var array = _array;
            var length = _length - 1;

            if (index < length)
            {
                Array.Copy(array, index + 1, array, index, length - index);
            }

            if (_referenceValues)
            {
                array[length] = default;
            }

            _length = length;
        }

        public void Sort(IComparer<T> comparer)
        {
            if (_length == 0) return;
            Array.Sort(_array, 0, _length, comparer);
        }

        public T[] ToArray()
        {
            if (_length == 0) return Array.Empty<T>();

            var result = new T[_length];
            Array.Copy(_array, result, _length);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WeakClear()
        {
            _length = 0;
        }

        public ref readonly T this[int index] => ref _array[index];

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithResize(T element)
        {
            var length = _length;

            var newLength = _resizeRule(length);
            var newArray = new T[newLength];

            if (length > 0)
            {
                Array.Copy(_array, 0, newArray, 0, length);
            }

            newArray[length] = element;

            _array = newArray;
            _length = length + 1;
        }
    }
}