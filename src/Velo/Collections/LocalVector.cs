using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Velo.Utils;

namespace Velo.Collections
{
    public partial struct LocalVector<T> : IEnumerable<T>
    {
        public int Length => _length;

        private T _element0;
        private T _element1;
        private T _element2;
        private T _element3;
        private T _element4;
        private Sequence<T> _sequence;

        private int _length;

        public LocalVector(int capacity)
        {
            _element0 = default;
            _element1 = default;
            _element2 = default;
            _element3 = default;
            _element4 = default;
            _sequence = capacity > 5 ? new Sequence<T>(capacity - 5) : null;
            
            _length = 0;
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
                default:
                    if (_sequence == null) _sequence = new Sequence<T>();
                    _sequence.Add(element);
                    break;
            }

            _length++;
        }

        public readonly bool Any(Predicate<T> predicate)
        {
            for (var i = 0; i < _length; i++)
                if (predicate(GetByIndex(i)))
                    return true;

            return false;
        }

        public bool Any<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            for (var i = 0; i < _length; i++)
                if (predicate(GetByIndex(i), arg))
                    return true;

            return false;
        }

        public void Clear()
        {
            _element0 = default;
            _element1 = default;
            _element2 = default;
            _element3 = default;
            _element4 = default;

            _sequence?.Clear();

            _length = 0;
        }

        public readonly bool Contains(T element, EqualityComparer<T> comparer = null)
        {
            if (comparer == null) comparer = EqualityComparer<T>.Default;

            for (var i = 0; i < _length; i++)
                if (comparer.Equals(GetByIndex(i), element))
                    return true;

            return false;
        }

        public readonly T First(Predicate<T> predicate)
        {
            for (var i = 0; i < _length; i++)
                if (predicate(GetByIndex(i)))
                    return this[i];

            throw Error.NotFound();
        }

        public readonly T First<TArg>(Func<T, TArg, bool> predicate, TArg arg)
        {
            for (var i = 0; i < _length; i++)
                if (predicate(GetByIndex(i), arg))
                    return this[i];

            throw Error.NotFound();
        }

        #region GetEnumerator

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Enumerator GetEnumerator()
        {
            return new Enumerator(_element0, _element1, _element2, _element3, _element4, _sequence, _length);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _length == 0 ? EmptyEnumerator<T>.Instance : GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _length == 0 ? EmptyEnumerator<T>.Instance : GetEnumerator();
        }

        #endregion

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
            }

            var result = new T[_length];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = GetByIndex(i);
            }

            return result;
        }

        public readonly T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            get
            {
                if (index > _length) throw Error.OutOfRange();
                return GetByIndex(index);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly T GetByIndex(int index)
        {
            switch (index)
            {
                case 0: return _element0;
                case 1: return _element1;
                case 2: return _element2;
                case 3: return _element3;
                case 4: return _element4;
                default: return _sequence[index - 5];
            }
        }
    }
}