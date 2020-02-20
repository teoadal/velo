using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.Collections
{
    [DebuggerTypeProxy(typeof(LocalListDebugVisualizer<>))]
    [DebuggerDisplay("Length = {" + nameof(_length) + "}")]
    public ref partial struct LocalList<T>
    {
        private const int Capacity = 10;

        public readonly int Length => _length;

        private T _element0;
        private T _element1;
        private T _element2;
        private T _element3;
        private T _element4;
        private T _element5;
        private T _element6;
        private T _element7;
        private T _element8;
        private T _element9;
        private T[] _array;

        private int _length;

        #region Constructors

        public LocalList(int capacity)
        {
            _element0 = default;
            _element1 = default;
            _element2 = default;
            _element3 = default;
            _element4 = default;
            _element5 = default;
            _element6 = default;
            _element7 = default;
            _element8 = default;
            _element9 = default;

            _array = capacity > Capacity ? new T[capacity - Capacity] : null;

            _length = 0;
        }

        public LocalList(T item0, T item1)
            : this(2)
        {
            _element0 = item0;
            _element1 = item1;

            _length = 2;
        }

        public LocalList(T item0, T item1, T item2)
            : this(3)
        {
            _element0 = item0;
            _element1 = item1;
            _element2 = item2;

            _length = 3;
        }

        public LocalList(T[] collection)
            : this(collection.Length)
        {
            foreach (var element in collection)
            {
                Set(_length, element);
                _length++;
            }
        }

        public LocalList(ICollection<T> collection)
            : this(collection.Count)
        {
            foreach (var element in collection)
            {
                Set(_length, element);
                _length++;
            }
        }

        public LocalList(IEnumerable<T> collection, int count)
            : this(count)
        {
            foreach (var element in collection)
            {
                Set(_length, element);
                _length++;
            }
        }
        
        #endregion

        public readonly bool All(Predicate<T> predicate)
        {
            for (var i = 0; i < _length; i++)
            {
                if (!predicate(Get(i)))
                {
                    return false;
                }
            }

            return true;
        }

        public void Add(T element)
        {
            switch (_length)
            {
                case 0:
                    _element0 = element;
                    break;
                case 1:
                    _element1 = element;
                    break;
                case 2:
                    _element2 = element;
                    break;
                case 3:
                    _element3 = element;
                    break;
                case 4:
                    _element4 = element;
                    break;
                case 5:
                    _element5 = element;
                    break;
                case 6:
                    _element6 = element;
                    break;
                case 7:
                    _element7 = element;
                    break;
                case 8:
                    _element8 = element;
                    break;
                case 9:
                    _element9 = element;
                    break;
                case Capacity:
                    if (_array == null) _array = new T[4];
                    _array[0] = element;
                    break;
                default:
                    AddToArray(element);
                    break;
            }

            _length++;
        }

        public void AddRange(LocalList<T> collection)
        {
            foreach (var element in collection)
            {
                Add(element);
            }
        }

        public readonly bool Any(Predicate<T> predicate)
        {
            for (var i = 0; i < _length; i++)
            {
                if (predicate(Get(i)))
                {
                    return true;
                }
            }

            return false;
        }

        public readonly bool Any<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            for (var i = 0; i < _length; i++)
            {
                if (predicate(Get(i), arg))
                {
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            _length = 0;
        }

        public readonly bool Contains(T element, EqualityComparer<T> comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < _length; i++)
            {
                if (comparer.Equals(Get(i), element))
                {
                    return true;
                }
            }

            return false;
        }

        public readonly T First(Predicate<T> predicate)
        {
            for (var i = 0; i < _length; i++)
            {
                var element = Get(i);
                if (predicate(element))
                {
                    return element;
                }
            }

            throw Error.NotFound();
        }

        public readonly T First<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            for (var i = 0; i < _length; i++)
            {
                var element = Get(i);
                if (predicate(element, arg))
                {
                    return element;
                }
            }

            throw Error.NotFound();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator GetEnumerator()
        {
            return new Enumerator(in this);
        }

        public readonly GroupEnumerator<TKey> GroupBy<TKey>(Func<T, TKey> keySelector,
            EqualityComparer<TKey> keyComparer = null)
        {
            if (keyComparer == null) keyComparer = EqualityComparer<TKey>.Default;
            return new GroupEnumerator<TKey>(this, keySelector, keyComparer);
        }

        public readonly int IndexOf(T element, EqualityComparer<T> comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            for (var index = 0; index < _length; index++)
            {
                if (comparer.Equals(element, Get(index)))
                {
                    return index;
                }
            }

            return -1;
        }

        public LocalList<T> OrderBy<TProperty>(Func<T, TProperty> property, Comparer<TProperty> comparer = null)
        {
            Sort(property, comparer);
            return this;
        }

        public readonly JoinEnumerator<TResult, TInner, TKey> Join<TResult, TInner, TKey>(
            LocalList<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultBuilder,
            EqualityComparer<TKey> keyComparer = null)
        {
            if (keyComparer == null) keyComparer = EqualityComparer<TKey>.Default;

            return new JoinEnumerator<TResult, TInner, TKey>(keyComparer,
                inner.GetEnumerator(), innerKeySelector,
                GetEnumerator(), outerKeySelector,
                resultBuilder);
        }

        public bool Remove(T element, EqualityComparer<T> comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            var index = IndexOf(element, comparer);
            if (index < 0) return false;

            RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (index >= _length) throw Error.OutOfRange();

            for (var i = index + 1; i < _length; i++)
            {
                Set(i - 1, Get(i));
            }

            _length--;
        }

        public void Sort<TProperty>(Func<T, TProperty> property, Comparer<TProperty> comparer = null)
        {
            if (comparer == null) comparer = Comparer<TProperty>.Default;

            var border = _length - 1;
            for (var i = 0; i < border; i++)
            {
                var final = false;
                for (var j = 0; j < border - i; j++)
                {
                    var current = Get(j);
                    var nextIndex = j + 1;
                    var next = Get(nextIndex);

                    if (comparer.Compare(property(next), property(current)) != -1) continue;

                    final = true;
                    Set(nextIndex, current);
                    Set(j, next);
                }

                if (!final) break;
            }
        }

        public readonly SelectEnumerator<TValue> Select<TValue>(Func<T, TValue> selector)
        {
            return new SelectEnumerator<TValue>(GetEnumerator(), selector);
        }

        public readonly SelectEnumerator<TValue, TArg> Select<TValue, TArg>(Func<T, TArg, TValue> selector, TArg arg)
        {
            return new SelectEnumerator<TValue, TArg>(GetEnumerator(), selector, arg);
        }

        public readonly int Sum(Func<T, int> selector)
        {
            var sum = 0;

            for (var i = 0; i < _length; i++)
            {
                var element = Get(i);
                sum += selector(element);
            }

            return sum;
        }

        public readonly WhereEnumerator Where(Predicate<T> predicate)
        {
            return new WhereEnumerator(GetEnumerator(), predicate);
        }

        public readonly WhereEnumerator<TArg> Where<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            return new WhereEnumerator<TArg>(GetEnumerator(), predicate, arg);
        }

        public readonly T[] ToArray()
        {
            var result = new T[_length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = Get(i);
            }

            return result;
        }

        public readonly T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= _length) throw Error.OutOfRange();
                return Get(index);
            }
        }

        private void AddToArray(T element)
        {
            var array = _array;
            var arrayIndex = _length - Capacity;

            if ((uint) arrayIndex < (uint) array.Length)
            {
                array[arrayIndex] = element;
            }
            else
            {
                var newArray = new T[array.Length * 2];
                Array.Copy(array, 0, newArray, 0, arrayIndex);

                newArray[arrayIndex] = element;

                _array = newArray;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly T Get(int index)
        {
            switch (index)
            {
                case 0: return _element0;
                case 1: return _element1;
                case 2: return _element2;
                case 3: return _element3;
                case 4: return _element4;
                case 5: return _element5;
                case 6: return _element6;
                case 7: return _element7;
                case 8: return _element8;
                case 9: return _element9;
                default:
                    return _array[index - Capacity];
            }
        }

        private void Set(int index, T value)
        {
            switch (index)
            {
                case 0:
                    _element0 = value;
                    return;
                case 1:
                    _element1 = value;
                    return;
                case 2:
                    _element2 = value;
                    return;
                case 3:
                    _element3 = value;
                    return;
                case 4:
                    _element4 = value;
                    return;
                case 5:
                    _element5 = value;
                    return;
                case 6:
                    _element6 = value;
                    return;
                case 7:
                    _element7 = value;
                    return;
                case 8:
                    _element8 = value;
                    return;
                case 9:
                    _element9 = value;
                    return;
                default:
                    _array[index - Capacity] = value;
                    return;
            }
        }
    }
}