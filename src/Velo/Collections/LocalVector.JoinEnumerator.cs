using System;
using System.Collections.Generic;

namespace Velo.Collections
{
    public ref partial struct LocalVector<T>
    {
        public ref struct JoinEnumerator<TResult, TInner, TKey>
        {
            public readonly TResult Current => _current!;

            // ReSharper disable FieldCanBeMadeReadOnly.Local
            private EqualityComparer<TKey> _comparer;

            private TResult _current;

            private LocalVector<TInner>.Enumerator _inner;
            private Func<TInner, TKey> _innerKeySelector;

            private Enumerator _outer;
            private T _outerCurrent;
            private TKey _outerKey;
            private Func<T, TKey> _outerKeySelector;
            private bool _outerMove;

            private Func<T, TInner, TResult> _resultBuilder;
            // ReSharper restore FieldCanBeMadeReadOnly.Local

            internal JoinEnumerator(EqualityComparer<TKey> comparer, LocalVector<TInner> inner,
                Func<TInner, TKey> innerKeySelector, Enumerator outer, Func<T, TKey> outerKeySelector,
                Func<T, TInner, TResult> resultBuilder)
            {
                _comparer = comparer;
                _current = default;

                _inner = inner.GetEnumerator();
                _innerKeySelector = innerKeySelector;

                _outer = outer;
                _outerCurrent = default;
                _outerKey = default;
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

            public LocalVector<TResult>.GroupEnumerator<TGroupKey> GroupBy<TGroupKey>(
                Func<TResult, TGroupKey> keySelector,
                EqualityComparer<TGroupKey> keyComparer = null)
            {
                var vector = new LocalVector<TResult>();

                while (MoveNext())
                {
                    vector.Add(_current);
                }

                if (keyComparer == null) keyComparer = EqualityComparer<TGroupKey>.Default;
                return new LocalVector<TResult>.GroupEnumerator<TGroupKey>(vector, keySelector, keyComparer);
            }

            public LocalVector<TResult> OrderBy<TProperty>(Func<TResult, TProperty> path,
                Comparer<TProperty> comparer = null)
            {
                var vector = new LocalVector<TResult>();

                while (MoveNext())
                {
                    vector.Add(_current);
                }

                vector.Sort(path, comparer);
                return vector;
            }

            public LocalVector<TValue> Select<TValue>(Func<TResult, TValue> selector)
            {
                var vector = new LocalVector<TValue>();

                while (MoveNext())
                {
                    vector.Add(selector(_current));
                }

                return vector;
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
                var vector = new LocalVector<TResult>();

                while (MoveNext())
                {
                    vector.Add(_current);
                }

                return vector.ToArray();
            }

            public LocalVector<TResult> Where(Predicate<TResult> predicate)
            {
                var vector = new LocalVector<TResult>();

                while (MoveNext())
                {
                    if (predicate(_current))
                    {
                        vector.Add(_current);
                    }
                }

                return vector;
            }

            public LocalVector<TResult> Where<TArg>(Func<TResult, TArg, bool> predicate, TArg arg)
            {
                var vector = new LocalVector<TResult>();

                while (MoveNext())
                {
                    if (predicate(_current, arg))
                    {
                        vector.Add(_current);
                    }
                }

                return vector;
            }
        }
    }
}