using System.Collections;
using System.Collections.Generic;

namespace Velo.Collections.Enumerators
{
    internal sealed class EmptyEnumerator<T> : IEnumerator<T>, IEnumerable<T>
    {
        public static readonly EmptyEnumerator<T> Instance = new EmptyEnumerator<T>();

        public T Current => default!;

        private EmptyEnumerator()
        {
        }

        public IEnumerator<T> GetEnumerator() => this;

        public bool MoveNext()
        {
            return false;
        }


        public void Dispose()
        {
        }

        void IEnumerator.Reset()
        {
        }

        object IEnumerator.Current => default!;
        IEnumerator IEnumerable.GetEnumerator() => this;
    }
}