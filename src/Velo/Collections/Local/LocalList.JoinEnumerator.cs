using System;
using System.Collections.Generic;

namespace Velo.Collections.Local
{
    public ref partial struct LocalList<T>
    {
        public ref struct JoinEnumerator<TResult, TInner, TKey>
        {
            public readonly TResult Current => _current!;

            // ReSharper disable FieldCanBeMadeReadOnly.Local
            private EqualityComparer<TKey> _comparer;

            private TResult _current;

            private LocalList<TInner>.Enumerator _inner;
            private Func<TInner, TKey> _innerKeySelector;

            private Enumerator _outer;
            private T _outerCurrent;
            private TKey _outerKey;
            private Func<T, TKey> _outerKeySelector;
            private bool _outerMove;

            private Func<T, TInner, TResult> _resultBuilder;
            // ReSharper restore FieldCanBeMadeReadOnly.Local

            internal JoinEnumerator(EqualityComparer<TKey> comparer, LocalList<TInner>.Enumerator inner,
                Func<TInner, TKey> innerKeySelector, Enumerator outer, Func<T, TKey> outerKeySelector,
                Func<T, TInner, TResult> resultBuilder)
            {
                _comparer = comparer;
                _current = default!;

                _inner = inner;
                _innerKeySelector = innerKeySelector;

                _outer = outer;
                _outerCurrent = default!;
                _outerKey = default!;
                _outerKeySelector = outerKeySelector;
                _outerMove = true;

                _resultBuilder = resultBuilder;
            }

            public readonly JoinEnumerator<TResult, TInner, TKey> GetEnumerator() => this;

            public bool MoveNext()
            {
                while (true)
                {
                    if (_outerMove)
                    {
                        if (!_outer.MoveNext()) return false;

                        _outerCurrent = _outer.Current;
                        _outerKey = _outerKeySelector(_outerCurrent);

                        _outerMove = false;
                    }

                    while (_inner.MoveNext())
                    {
                        var inner = _inner.Current;
                        var innerKey = _innerKeySelector(inner);

                        if (!_comparer.Equals(_outerKey, innerKey)) continue;

                        _current = _resultBuilder(_outerCurrent, inner);

                        return true;
                    }

                    _inner.Reset();
                    _outerMove = true;
                }
            }

            public LocalList<TResult>.GroupEnumerator<TGroupKey> GroupBy<TGroupKey>(
                Func<TResult, TGroupKey> keySelector,
                EqualityComparer<TGroupKey>? keyComparer = null)
            {
                var localList = new LocalList<TResult>();

                while (MoveNext())
                {
                    localList.Add(_current);
                }

                keyComparer ??= EqualityComparer<TGroupKey>.Default;
                return new LocalList<TResult>.GroupEnumerator<TGroupKey>(localList, keySelector, keyComparer);
            }

            public LocalList<TResult> OrderBy<TProperty>(Func<TResult, TProperty> path,
                Comparer<TProperty>? comparer = null)
            {
                var localList = new LocalList<TResult>();

                while (MoveNext())
                {
                    localList.Add(_current);
                }

                localList.Sort(path, comparer);
                return localList;
            }

            public LocalList<TValue> Select<TValue>(Func<TResult, TValue> selector)
            {
                var localList = new LocalList<TValue>();

                while (MoveNext())
                {
                    localList.Add(selector(_current));
                }

                return localList;
            }

            public int Sum(Func<TResult, int> selector)
            {
                var sum = 0;

                while (MoveNext())
                {
                    sum += selector(_current);
                }

                return sum;
            }

            public TResult[] ToArray()
            {
                var localList = new LocalList<TResult>();

                while (MoveNext())
                {
                    localList.Add(_current);
                }

                return localList.ToArray();
            }

            public LocalList<TResult> Where(Predicate<TResult> predicate)
            {
                var localList = new LocalList<TResult>();

                while (MoveNext())
                {
                    if (predicate(_current))
                    {
                        localList.Add(_current);
                    }
                }

                return localList;
            }

            public LocalList<TResult> Where<TArg>(Func<TResult, TArg, bool> predicate, TArg arg)
            {
                var localList = new LocalList<TResult>();

                while (MoveNext())
                {
                    if (predicate(_current, arg))
                    {
                        localList.Add(_current);
                    }
                }

                return localList;
            }
        }
    }
}