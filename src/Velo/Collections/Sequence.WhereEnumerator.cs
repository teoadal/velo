using System;
using System.Collections;
using System.Collections.Generic;

namespace Velo.Collections
{
    public sealed partial class Sequence<T>
    {
        public struct WhereEnumerator : IEnumerator<T>, IEnumerable<T>
        {
            public T Current { get; private set; }

            private T[] _array;
            private readonly uint _length;
            private Predicate<T> _predicate;

            private int _index;

            internal WhereEnumerator(T[] items, int length, Predicate<T> predicate)
            {
                _array = items;
                _length = (uint) length;
                _predicate = predicate;

                _index = 0;

                Current = default;
            }

            public WhereEnumerator GetEnumerator() => this;

            public bool MoveNext()
            {
                while ((uint) _index < _length)
                {
                    var item = _array[_index++];
                    if (!_predicate(item)) continue;

                    Current = item;
                    return true;
                }

                return false;
            }

            void IEnumerator.Reset()
            {
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;

            IEnumerator IEnumerable.GetEnumerator() => this;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                Current = default;
                _array = null;
                _predicate = null;
            }
        }

        public struct WhereEnumerator<TArg> : IEnumerator<T>, IEnumerable<T>
        {
            public T Current { get; private set; }

            private TArg _arg;
            private T[] _array;
            private readonly uint _length;
            private Func<T, TArg, bool> _predicate;

            private int _index;

            internal WhereEnumerator(T[] items, int length, Func<T, TArg, bool> predicate, TArg arg)
            {
                _array = items;
                _length = (uint) length;
                _predicate = predicate;
                _arg = arg;

                _index = 0;

                Current = default;
            }

            public WhereEnumerator<TArg> GetEnumerator() => this;

            public bool MoveNext()
            {
                while ((uint) _index < _length)
                {
                    var item = _array[_index++];
                    if (!_predicate(item, _arg)) continue;

                    Current = item;
                    return true;
                }

                return false;
            }

            void IEnumerator.Reset()
            {
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator() => this;

            IEnumerator IEnumerable.GetEnumerator() => this;

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                Current = default;
                _arg = default;
                _array = null;
                _predicate = null;
            }
        }
    }
}