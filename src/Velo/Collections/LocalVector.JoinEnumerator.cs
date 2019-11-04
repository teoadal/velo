using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Velo.Collections
{
    public ref partial struct LocalVector<T>
    {
        public ref struct JoinEnumerator<TResult, TInner, TKey>
        {
            public TResult Current => _current!;

            private readonly EqualityComparer<TKey> _comparer;
            private TResult _current;
            private LocalVector<TInner>.Enumerator _inner;
            private readonly Func<TInner, TKey> _innerKeySelector;
            private Enumerator _outer;
            private readonly Func<T, TKey> _outerKeySelector;
            private readonly Func<T, TInner, TResult> _resultBuilder;

            public JoinEnumerator(EqualityComparer<TKey> comparer, LocalVector<TInner>.Enumerator inner,
                Func<TInner, TKey> innerKeySelector, Enumerator outer, Func<T, TKey> outerKeySelector,
                Func<T, TInner, TResult> resultBuilder)
            {
                _comparer = comparer;
                _current = default;
                _inner = inner;
                _innerKeySelector = innerKeySelector;
                _outer = outer;
                _outerKeySelector = outerKeySelector;
                _resultBuilder = resultBuilder;
            }

            public JoinEnumerator<TResult, TInner, TKey> GetEnumerator() => this;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                while (_outer.MoveNext())
                {
                    var outerCurrent = _outer.Current;
                    var outerKey = _outerKeySelector(outerCurrent);

                    while (_inner.MoveNext())
                    {
                        var innerCurrent = _inner.Current;
                        var innerKey = _innerKeySelector(innerCurrent);

                        if (!_comparer.Equals(outerKey, innerKey)) continue;

                        _current = _resultBuilder(outerCurrent, innerCurrent);
                        return true;
                    }
                    
                    _inner.Reset();
                }

                return false;
            }

            public LocalVector<TResult> OrderBy<TProperty>(Func<TResult, TProperty> path, Comparer<TProperty> comparer = null)
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
                    var current = _current;
                    if (predicate(current))
                    {
                        vector.Add(current);    
                    }
                }

                return vector;
            }
            
            public LocalVector<TResult> Where<TArg>(Func<TResult, TArg, bool> predicate, TArg arg)
            {
                var vector = new LocalVector<TResult>();
                while (MoveNext())
                {
                    var current = _current;
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