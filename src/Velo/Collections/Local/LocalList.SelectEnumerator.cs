using System;
using System.Collections.Generic;

namespace Velo.Collections.Local
{
    public ref partial struct LocalList<T>
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
                _current = default!;
                _enumerator = enumerator;
                _selector = selector;
            }

            public readonly SelectEnumerator<TValue> GetEnumerator() => this;

            public LocalList<TValue>.JoinEnumerator<TResult, TInner, TKey> Join<TResult, TInner, TKey>(
                LocalList<TInner> inner,
                Func<TValue, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<TValue, TInner, TResult> resultBuilder,
                EqualityComparer<TKey>? keyComparer = null)
            {
                var outer = new LocalList<TValue>();

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

            public LocalList<TValue> OrderBy<TProperty>(Func<TValue, TProperty> path,
                Comparer<TProperty>? comparer = null)
            {
                var localList = new LocalList<TValue>();

                while (_enumerator.MoveNext())
                {
                    localList.Add(_selector(_enumerator.Current));
                }

                localList.Sort(path, comparer);
                return localList;
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

            public LocalList<TValue> Where(Predicate<TValue> predicate)
            {
                var localList = new LocalList<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _selector(_enumerator.Current);
                    if (predicate(current))
                    {
                        localList.Add(current);
                    }
                }

                return localList;
            }

            public LocalList<TValue> Where<TArg>(Func<TValue, TArg, bool> predicate, TArg arg)
            {
                var localList = new LocalList<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _selector(_enumerator.Current);
                    if (predicate(current, arg))
                    {
                        localList.Add(current);
                    }
                }

                return localList;
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
                _current = default!;
                _enumerator = enumerator;
                _selector = selector;
            }

            public readonly SelectEnumerator<TValue, TArg> GetEnumerator() => this;

            public LocalList<TValue>.JoinEnumerator<TResult, TInner, TKey> Join<TResult, TInner, TKey>(
                LocalList<TInner> inner,
                Func<TValue, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<TValue, TInner, TResult> resultBuilder,
                EqualityComparer<TKey>? keyComparer = null)
            {
                var outer = new LocalList<TValue>();
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

            public LocalList<TValue> OrderBy<TProperty>(Func<TValue, TProperty> path,
                Comparer<TProperty>? comparer = null)
            {
                var localList = new LocalList<TValue>();

                while (_enumerator.MoveNext())
                {
                    localList.Add(_selector(_enumerator.Current, _arg));
                }

                localList.Sort(path, comparer);
                return localList;
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

            public LocalList<TValue> Where(Predicate<TValue> predicate)
            {
                var localList = new LocalList<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _selector(_enumerator.Current, _arg);
                    if (predicate(current))
                    {
                        localList.Add(current);
                    }
                }

                return localList;
            }

            public LocalList<TValue> Where<TWhereArg>(Func<TValue, TWhereArg, bool> predicate, TWhereArg arg)
            {
                var localList = new LocalList<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _selector(_enumerator.Current, _arg);
                    if (predicate(current, arg))
                    {
                        localList.Add(current);
                    }
                }

                return localList;
            }
        }
    }
}