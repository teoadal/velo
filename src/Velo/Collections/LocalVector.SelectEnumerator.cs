using System;
using System.Collections.Generic;

namespace Velo.Collections
{
    public ref partial struct LocalVector<T>
    {
        public ref struct SelectEnumerator<TValue>
        {
            public readonly TValue Current => _current!;

            // ReSharper disable FieldCanBeMadeReadOnly.Local
            private TValue _current;
            private Enumerator _enumerator;
            private Func<T, TValue> _selector;
            // ReSharper restore FieldCanBeMadeReadOnly.Local

            internal SelectEnumerator(Enumerator enumerator, Func<T, TValue> selector)
            {
                _current = default;
                _enumerator = enumerator;
                _selector = selector;
            }

            public readonly SelectEnumerator<TValue> GetEnumerator() => this;

            public LocalVector<TValue>.JoinEnumerator<TResult, TInner, TKey> Join<TResult, TInner, TKey>(
                LocalVector<TInner> inner,
                Func<TValue, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<TValue, TInner, TResult> resultBuilder,
                EqualityComparer<TKey> keyComparer = null)
            {
                var outer = new LocalVector<TValue>();

                while (_enumerator.MoveNext())
                {
                    outer.Add(_selector(_enumerator.Current));
                }

                return outer.Join(inner, outerKeySelector, innerKeySelector, resultBuilder, keyComparer);
            }

            public bool MoveNext()
            {
                while (_enumerator.MoveNext())
                {
                    _current = _selector(_enumerator.Current);
                    return true;
                }

                return false;
            }

            public LocalVector<TValue> OrderBy<TProperty>(Func<TValue, TProperty> path,
                Comparer<TProperty> comparer = null)
            {
                var vector = new LocalVector<TValue>();

                while (_enumerator.MoveNext())
                {
                    vector.Add(_selector(_enumerator.Current));
                }

                vector.Sort(path, comparer);
                return vector;
            }

            public TValue[] ToArray()
            {
                var array = new TValue[_enumerator.Length];

                var counter = 0;
                while (_enumerator.MoveNext())
                {
                    array[counter++] = _selector(_enumerator.Current);
                }

                return array;
            }

            public LocalVector<TValue> Where(Predicate<TValue> predicate)
            {
                var vector = new LocalVector<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _selector(_enumerator.Current);
                    if (predicate(current))
                    {
                        vector.Add(current);
                    }
                }

                return vector;
            }

            public LocalVector<TValue> Where<TArg>(Func<TValue, TArg, bool> predicate, TArg arg)
            {
                var vector = new LocalVector<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _selector(_enumerator.Current);
                    if (predicate(current, arg))
                    {
                        vector.Add(current);
                    }
                }

                return vector;
            }
        }

        public ref struct SelectEnumerator<TValue, TArg>
        {
            public TValue Current => _current!;

            // ReSharper disable FieldCanBeMadeReadOnly.Local
            private TArg _arg;
            private TValue _current;
            private Enumerator _enumerator;
            private Func<T, TArg, TValue> _selector;
            // ReSharper restore FieldCanBeMadeReadOnly.Local

            public SelectEnumerator(Enumerator enumerator, Func<T, TArg, TValue> selector, TArg arg)
            {
                _arg = arg;
                _current = default;
                _enumerator = enumerator;
                _selector = selector;
            }

            public readonly SelectEnumerator<TValue, TArg> GetEnumerator() => this;

            public LocalVector<TValue>.JoinEnumerator<TResult, TInner, TKey> Join<TResult, TInner, TKey>(
                LocalVector<TInner> inner,
                Func<TValue, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<TValue, TInner, TResult> resultBuilder,
                EqualityComparer<TKey> keyComparer = null)
            {
                var outer = new LocalVector<TValue>();
                while (MoveNext())
                {
                    outer.Add(_current);
                }

                return outer.Join(inner, outerKeySelector, innerKeySelector, resultBuilder, keyComparer);
            }

            public bool MoveNext()
            {
                while (_enumerator.MoveNext())
                {
                    _current = _selector(_enumerator.Current, _arg);
                    return true;
                }

                return false;
            }

            public LocalVector<TValue> OrderBy<TProperty>(Func<TValue, TProperty> path,
                Comparer<TProperty> comparer = null)
            {
                var vector = new LocalVector<TValue>();

                while (_enumerator.MoveNext())
                {
                    vector.Add(_selector(_enumerator.Current, _arg));
                }

                vector.Sort(path, comparer);
                return vector;
            }

            public TValue[] ToArray()
            {
                var array = new TValue[_enumerator.Length];

                var counter = 0;
                while (_enumerator.MoveNext())
                {
                    array[counter++] = _selector(_enumerator.Current, _arg);
                }

                return array;
            }

            public LocalVector<TValue> Where(Predicate<TValue> predicate)
            {
                var vector = new LocalVector<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _selector(_enumerator.Current, _arg);
                    if (predicate(current))
                    {
                        vector.Add(current);
                    }
                }

                return vector;
            }

            public LocalVector<TValue> Where<TWhereArg>(Func<TValue, TWhereArg, bool> predicate, TWhereArg arg)
            {
                var vector = new LocalVector<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _selector(_enumerator.Current, _arg);
                    if (predicate(current, arg))
                    {
                        vector.Add(current);
                    }
                }

                return vector;
            }
        }
    }
}