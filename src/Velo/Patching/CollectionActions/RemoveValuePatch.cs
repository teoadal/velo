using System;
using System.Collections.Generic;

namespace Velo.Patching.CollectionActions
{
    internal sealed class RemoveValuePatch<T, TValue> : IPatchAction<T>
        where T : class
    {
        private readonly Func<T, ICollection<TValue>> _getter;
        private readonly TValue _value;

        public RemoveValuePatch(Func<T, ICollection<TValue>> getter, TValue value)
        {
            _getter = getter;
            _value = value;
        }

        public void Apply(T instance)
        {
            var collection = _getter(instance);
            collection?.Remove(_value);
        }
    }
}