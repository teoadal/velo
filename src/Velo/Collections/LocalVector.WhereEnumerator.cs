using System;
using System.Collections;
using System.Collections.Generic;

namespace Velo.Collections
{
    public ref partial struct LocalVector<T>
    {
        public ref struct WhereEnumerator
        {
            public T Current => _current!;

            private T _current;
            private Enumerator _enumerator;
            private readonly Predicate<T> _predicate;

            public WhereEnumerator(Enumerator enumerator, Predicate<T> predicate)
            {
                _current = default;
                _enumerator = enumerator;
                _predicate = predicate;
            }

            public WhereEnumerator GetEnumerator() => this;

            public JoinEnumerator<TResult, TInner, TKey> Join<TResult, TInner, TKey>(
                LocalVector<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultBuilder,
                EqualityComparer<TKey> keyComparer = null)
            {
                var outer = new LocalVector<T>();
                while (MoveNext())
                {
                    outer.Add(_current);
                }

                return outer.Join(inner, outerKeySelector, innerKeySelector, resultBuilder, keyComparer);
            }
            
            public bool MoveNext()
            {
                var predicate = _predicate;

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (!predicate(current)) continue;

                    _current = current;
                    return true;
                }

                return false;
            }

            public LocalVector<T> OrderBy<TProperty>(Func<T, TProperty> path, Comparer<TProperty> comparer = null)
            {
                var vector = new LocalVector<T>();
                
                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current))
                    {
                        vector.Add(current);    
                    }
                }
                
                vector.Sort(path, comparer);
                return vector;
            }
            
            public LocalVector<TValue> Select<TValue>(Func<T, TValue> selector)
            {
                var predicate = _predicate;
                var vector = new LocalVector<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (predicate(current))
                    {
                        vector.Add(selector(current));
                    }
                }

                return vector;
            }
            
            public T[] ToArray()
            {
                var list = new List<T>(_enumerator.Length);
                
                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current))
                    {
                        list.Add(current);
                    }
                }

                return list.ToArray();
            }
        }

        public ref struct WhereEnumerator<TArg>
        {
            public T Current => _current!;

            private readonly TArg _arg;
            private T _current;
            private Enumerator _enumerator;
            private readonly Func<T, TArg, bool> _predicate;

            public WhereEnumerator(Enumerator enumerator, Func<T, TArg, bool> predicate, TArg arg)
            {
                _arg = arg;
                _current = default;
                _enumerator = enumerator;
                _predicate = predicate;
            }

            public WhereEnumerator<TArg> GetEnumerator() => this;

            public JoinEnumerator<TResult, TInner, TKey> Join<TResult, TInner, TKey>(
                LocalVector<TInner> inner,
                Func<T, TKey> outerKeySelector,
                Func<TInner, TKey> innerKeySelector,
                Func<T, TInner, TResult> resultBuilder,
                EqualityComparer<TKey> keyComparer = null)
            {
                var outer = new LocalVector<T>();
                while (MoveNext())
                {
                    outer.Add(_current);
                }

                return outer.Join(inner, outerKeySelector, innerKeySelector, resultBuilder, keyComparer);
            }
            
            public bool MoveNext()
            {
                var arg = _arg;
                var predicate = _predicate;

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (!predicate(current, arg)) continue;

                    _current = current;
                    return true;
                }

                return false;
            }

            public LocalVector<T> OrderBy<TProperty>(Func<T, TProperty> path, Comparer<TProperty> comparer = null)
            {
                var vector = new LocalVector<T>();
                
                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current, _arg))
                    {
                        vector.Add(current);    
                    }
                }
                
                vector.Sort(path, comparer);
                return vector;
            }
            
            public LocalVector<TValue> Select<TValue>(Func<T, TValue> selector)
            {
                var arg = _arg;
                var predicate = _predicate;
                var vector = new LocalVector<TValue>();

                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (predicate(current, arg))
                    {
                        vector.Add(selector(current));
                    }
                }

                return vector;
            }
            
            public T[] ToArray()
            {
                var vector = new LocalVector<T>();
                
                while (_enumerator.MoveNext())
                {
                    var current = _enumerator.Current;
                    if (_predicate(current, _arg))
                    {
                        vector.Add(current);
                    }
                }

                return vector.ToArray();
            }
        }
    }
}