using System;
using Velo.Collections;

namespace Velo.Patching.ArrayActions
{
    internal sealed class ClearArrayValuesPath<T, TValue> : IPatchAction<T>
        where T: class
    {
        private readonly Func<T, TValue[]> _getter;
        private readonly Action<T, TValue[]> _setter;

        public ClearArrayValuesPath(Func<T, TValue[]> getter, Action<T,TValue[]> setter)
        {
            _getter = getter;
            _setter = setter;
        }

        public void Apply(T instance)
        {
            var array = _getter(instance);
            if (array.NullOrEmpty()) return;

            _setter(instance, Array.Empty<TValue>());
        }
    }
}