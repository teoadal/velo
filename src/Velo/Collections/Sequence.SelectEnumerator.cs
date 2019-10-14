using System;
using System.Collections;
using System.Collections.Generic;

namespace Velo.Collections
{
    public sealed partial class Sequence<T>
    {
        public struct SelectEnumerator<TValue> : IEnumerator<TValue>, IEnumerable<TValue>
        {
            public TValue Current { get; private set; }

            private T[] _array;
            private readonly uint _length;
            private Func<T, TValue> _selector;

            private int _index;
            
            internal SelectEnumerator(T[] array, int length, Func<T, TValue> selector)
            {
                Current = default;
                _array = array;
                _length = (uint) length;
                _index = 0;
                _selector = selector;
            }

            public SelectEnumerator<TValue> GetEnumerator() => this;
            
            public bool MoveNext()
            {
                if ((uint) _index >= _length) return false;

                Current = _selector(_array[_index++]);
                return true;
            }
            
            public TValue[] ToArray()
            {
                var result = new TValue[_length];
                for (var i = 0; i < result.Length; i++)
                {
                    if (i == _length) break;
                    result[i] = _selector(_array[i]);
                }

                Dispose();
                return result;
            }

            public void Dispose()
            {
                Current = default;
                _array = null;
                _selector = null;
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() => this;

            IEnumerator IEnumerable.GetEnumerator() => this;

            void IEnumerator.Reset()
            {
            }

            object IEnumerator.Current => Current;
        }
    }
}