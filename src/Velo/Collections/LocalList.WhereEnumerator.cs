using System;
using System.Collections.Generic;

namespace Velo.Collections
{
    public ref partial struct LocalList<T>
    {
        public ref struct WhereEnumerator
        {
            public readonly T Current => _current!;

            // ReSharper disable FieldCanBeMadeReadOnly.Local
            private T _current;
            private Enumerator _enumerator;
            private Predicate<T> _predicate;
            // ReSharper restore FieldCanBeMadeReadOnly.Local

            internal WhereEnumerator(Enumerator enumerator, Predicate<T> predicate)
            {
                _current = default;
                _enumerator = enumerator;
                _predicate = predicate;
            }

            public readonly WhereEnumerator GetEnumerator() => this;

            public JoinEnumerator<TResult, TInner, TKey> Join<TResult, TInner, TKey>(
                LocalList<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultBuilder,
                EqualityComparer<TKey> keyComparer = null)
            {
                var outer = new LocalList<T>();

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current))
                    {
                        outer.Add(current);
                    }
                }

                return outer.Join(inner, outerKeySelector, innerKeySelector, resultBuilder, keyComparer);
            }

            public bool MoveNext()
            {
                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (!_predicate(current)) continue;

                    _current = current;
                    return true;
                }

                return false;
            }

            public LocalList<T> OrderBy<TProperty>(Func<T, TProperty> path, Comparer<TProperty> comparer = null)
            {
                var localList = new LocalList<T>();

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current))
                    {
                        localList.Add(current);
                    }
                }

                localList.Sort(path, comparer);
                return localList;
            }

            public LocalList<TValue> Select<TValue>(Func<T, TValue> selector)
            {
                var localList = new LocalList<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current))
                    {
                        localList.Add(selector(current));
                    }
                }

                return localList;
            }

            public T[] ToArray()
            {
                var localList = new LocalList<T>(_enumerator.Length);

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current))
                    {
                        localList.Add(current);
                    }
                }

                return localList.ToArray();
            }
        }

        public ref struct WhereEnumerator<TArg>
        {
            public T Current => _current!;

            // ReSharper disable FieldCanBeMadeReadOnly.Local
            private TArg _arg;
            private T _current;
            private Enumerator _enumerator;
            private Func<T, TArg, bool> _predicate;
            // ReSharper restore FieldCanBeMadeReadOnly.Local

            public WhereEnumerator(Enumerator enumerator, Func<T, TArg, bool> predicate, TArg arg)
            {
                _arg = arg;
                _current = default;
                _enumerator = enumerator;
                _predicate = predicate;
            }

            public readonly WhereEnumerator<TArg> GetEnumerator() => this;

            public JoinEnumerator<TResult, TInner, TKey> Join<TResult, TInner, TKey>(
                LocalList<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultBuilder,
                EqualityComparer<TKey> keyComparer = null)
            {
                var outer = new LocalList<T>();
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
                    var current = _enumerator.Current;
                    if (!_predicate(current, _arg)) continue;

                    _current = current;
                    return true;
                }

                return false;
            }

            public LocalList<T> OrderBy<TProperty>(Func<T, TProperty> path, Comparer<TProperty> comparer = null)
            {
                var localList = new LocalList<T>();

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current, _arg))
                    {
                        localList.Add(current);
                    }
                }

                localList.Sort(path, comparer);
                return localList;
            }

            public LocalList<TValue> Select<TValue>(Func<T, TValue> selector)
            {
                var localList = new LocalList<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current, _arg))
                    {
                        localList.Add(selector(current));
                    }
                }

                return localList;
            }

            public LocalList<TValue> Select<TValue, TSelectArg>(Func<T, TSelectArg, TValue> selector, TSelectArg arg)
            {
                var localList = new LocalList<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current, _arg))
                    {
                        localList.Add(selector(current, arg));
                    }
                }

                return localList;
            }
            
            public T[] ToArray()
            {
                var localList = new LocalList<T>();

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current, _arg))
                    {
                        localList.Add(current);
                    }
                }

                return localList.ToArray();
            }
        }
    }
}