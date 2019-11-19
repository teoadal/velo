using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.Collections
{
    [DebuggerTypeProxy(typeof(LocalVectorDebugVisualizer<>))]
    [DebuggerDisplay("Length = {" + nameof(_length) + "}")]
    public ref partial struct LocalVector<T>
    {
        private const int Capacity = 6;

        public int Length => _length;

        private T _element0;
        private T _element1;
        private T _element2;
        private T _element3;
        private T _element4;
        private T _element5;
        private T[] _array;

        private int _length;

        #region Constructors

        public LocalVector(int capacity)
        {
            _element0 = default;
            _element1 = default;
            _element2 = default;
            _element3 = default;
            _element4 = default;
            _element5 = default;

            _array = capacity > Capacity ? new T[capacity - Capacity] : null;

            _length = 0;
        }

        public LocalVector(T[] collection)
            : this(collection.Length)
        {
            foreach (var element in collection)
            {
                Add(element);
            }
        }

        public LocalVector(ICollection<T> collection)
            : this(collection.Count)
        {
            foreach (var element in collection)
            {
                Add(element);
            }
        }

        #endregion

        public bool All(Predicate<T> predicate)
        {
            for (var i = 0; i < _length; i++)
                if (!predicate(Get(i)))
                    return false;

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
                default:
                    AddToArray(element);
                    break;
            }

            _length++;
        }

        public void AddRange(LocalVector<T> collection)
        {
            foreach (var element in collection)
                Add(element);
        }

        public readonly bool Any(Predicate<T> predicate)
        {
            for (var i = 0; i < _length; i++)
                if (predicate(Get(i)))
                    return true;

            return false;
        }

        public readonly bool Any<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            for (var i = 0; i < _length; i++)
                if (predicate(Get(i), arg))
                    return true;

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
                if (comparer.Equals(Get(i), element))
                    return true;

            return false;
        }

        public readonly T First(Predicate<T> predicate)
        {
            for (var i = 0; i < _length; i++)
                if (predicate(Get(i)))
                    return this[i];

            throw Error.NotFound();
        }

        public readonly T First<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            for (var i = 0; i < _length; i++)
                if (predicate(Get(i), arg))
                    return this[i];

            throw Error.NotFound();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public readonly GroupEnumerator<TKey> GroupBy<TKey>(Func<T, TKey> keySelector,
            EqualityComparer<TKey> keyComparer = null)
        {
            if (keyComparer == null) keyComparer = EqualityComparer<TKey>.Default;
            return new GroupEnumerator<TKey>(this, keySelector, keyComparer);
        }

        public JoinEnumerator<TResult, TInner, TKey> Join<TResult, TInner, TKey>(
            LocalVector<TInner> inner,
            Func<T, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<T, TInner, TResult> resultBuilder,
            EqualityComparer<TKey> keyComparer = null)
        {
            if (keyComparer == null) keyComparer = EqualityComparer<TKey>.Default;

            var innerEnumerator = inner.GetEnumerator();
            var outerEnumerator = GetEnumerator();

            return new JoinEnumerator<TResult, TInner, TKey>(keyComparer,
                innerEnumerator, innerKeySelector,
                outerEnumerator, outerKeySelector,
                resultBuilder);
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
            switch (_length)
            {
                case 0:
                    return Array.Empty<T>();
                case 1:
                    return new[] {_element0};
                case 2:
                    return new[] {_element0, _element1};
                case 3:
                    return new[] {_element0, _element1, _element2};
                case 4:
                    return new[] {_element0, _element1, _element2, _element3};
                case 5:
                    return new[] {_element0, _element1, _element2, _element3, _element4};
                case 6:
                    return new[] {_element0, _element1, _element2, _element3, _element4, _element5};
            }

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
            var length = _length;
            var array = _array;

            if (length == Capacity)
            {
                if (array == null) _array = array = new T[4];
                array[0] = element;
                return;
            }

            var index = length - Capacity;
            if ((uint) index < (uint) array.Length)
            {
                array[index] = element;
            }
            else
            {
                var newArray = new T[array.Length * 2];
                Array.Copy(array, 0, newArray, 0, index);

                newArray[index] = element;

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
                default: return _array[index - Capacity];
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
                default:
                    _array[index - Capacity] = value;
                    return;
            }
        }
    }
}