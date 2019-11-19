using System.Collections;
using System.Collections.Generic;

namespace Velo.Collections
{
    internal sealed class EmptyEnumerator<T> : IEnumerator<T>
    {
        public static readonly IEnumerator<T> Instance = new EmptyEnumerator<T>();

        private EmptyEnumerator()
        {
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        public T Current => default!;

        object IEnumerator.Current => default!;

        public void Dispose()
        {
        }
    }
}