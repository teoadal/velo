using System;
using System.Collections.Generic;

namespace Velo.Patching.CollectionActions
{
    internal sealed class ReplacePatch<T, TValue> : IPatchAction<T>
        where T : class
    {
        private readonly Func<T, IList<TValue>> _getter;

        private readonly TValue _oldValue;
        private readonly TValue _newValue;

        public ReplacePatch(Func<T, IList<TValue>> getter, TValue oldValue, TValue newValue)
        {
            _getter = getter;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public void Apply(T instance)
        {
            var collection = _getter(instance);

            var index = collection.IndexOf(_oldValue);
            if (index == -1) return;

            collection[index] = _newValue;
        }
    }
}