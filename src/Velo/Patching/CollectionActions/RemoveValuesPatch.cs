using System;
using System.Collections.Generic;

namespace Velo.Patching.CollectionActions
{
    internal sealed class RemoveValuesPatch<T, TValue> : IPatchAction<T>
        where T : class
    {
        private readonly Func<T, ICollection<TValue>> _getter;
        private readonly TValue[] _values;

        public RemoveValuesPatch(Func<T, ICollection<TValue>> getter, TValue[] values)
        {
            _getter = getter;
            _values = values;
        }

        public void Apply(T instance)
        {
            var collection = _getter(instance);

            if (collection == null) return;

            var values = _values;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                collection.Remove(value);
            }
        }
    }
}