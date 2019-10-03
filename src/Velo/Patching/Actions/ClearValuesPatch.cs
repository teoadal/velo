using System;
using System.Collections.Generic;

namespace Velo.Patching.Actions
{
    internal sealed class ClearValuesPatch<T, TValue>: IPatchAction<T> where T: class
    {
        private readonly Func<T, ICollection<TValue>> _getter;

        public ClearValuesPatch(Func<T,ICollection<TValue>> getter)
        {
            _getter = getter;
        }

        public void Apply(T instance)
        {
            var collection = _getter(instance);
            collection?.Clear();
        }
    }
}