using System;
using System.Collections.Generic;

namespace Velo.Collections.Local
{
    public ref partial struct LocalList<T>
    {
        public ref struct GroupEnumerator<TKey>
        {
            public readonly LocalGroup Current => _current;

            // ReSharper disable FieldCanBeMadeReadOnly.Local
            private EqualityComparer<TKey> _comparer;
            private LocalList<Row<TKey, T>> _values;
            private LocalList<TKey> _uniqueKeys;
            // ReSharper restore FieldCanBeMadeReadOnly.Local

            private LocalGroup _current;
            private int _position;

            internal GroupEnumerator(LocalList<T> list, Func<T, TKey> keySelector, EqualityComparer<TKey> comparer)
            {
                var values = new LocalList<Row<TKey, T>>(list.Length);
                var uniqueKeys = new LocalList<TKey>();

                for (var i = 0; i < list.Length; i++)
                {
                    var element = list.Get(i);
                    var key = keySelector(element);

                    values.Add(new Row<TKey, T>(key, element));

                    if (!uniqueKeys.Contains(key, comparer))
                    {
                        uniqueKeys.Add(key);
                    }
                }

                _comparer = comparer;
                _current = default;
                _position = 0;
                _values = values;
                _uniqueKeys = uniqueKeys;
            }

            public readonly GroupEnumerator<TKey> GetEnumerator() => this;

            public bool MoveNext()
            {
                if (_position >= _uniqueKeys.Length) return false;

                var currentKey = _uniqueKeys.Get(_position);

                var groupLocalList = new LocalList<T>();
                for (var i = 0; i < _values.Length; i++)
                {
                    var value = _values.Get(i);
                    if (_comparer.Equals(currentKey, value.Key))
                    {
                        groupLocalList.Add(value.Value);
                    }
                }

                _current = new LocalGroup(currentKey, groupLocalList);
                _position++;

                return true;
            }

            public LocalList<TResult> Select<TResult>(Selector<TResult> selector)
            {
                var result = new LocalList<TResult>(_uniqueKeys._length);
                while (MoveNext())
                {
                    var element = selector(_current);
                    result.Add(element);
                }

                return result;
            }

            public int Sum(Selector<int> selector)
            {
                var sum = 0;
                
                while (MoveNext())
                {
                    sum += selector(_current);
                }

                return sum;
            }
            
            public readonly ref struct LocalGroup
            {
                public readonly TKey Key;

                public readonly LocalList<T> Values;

                internal LocalGroup(TKey key, LocalList<T> values)
                {
                    Key = key;
                    Values = values;
                }
            }

            public delegate TResult Selector<out TResult>(LocalGroup group);
        }
    }
}