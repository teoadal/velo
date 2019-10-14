using System.Collections;
using System.Collections.Generic;

namespace Velo.Collections
{
    internal sealed class EmptyEnumerator<T> : IEnumerator<T>
    {
        public static readonly IEnumerator<T> Instance = new EmptyEnumerator<T>();

        private EmptyEnumerator()
        {
            Current = default;
        }
        
        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        public T Current { get; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }

}