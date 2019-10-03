using System;
using System.Collections.Generic;

namespace Velo.Patching.ArrayActions
{
    internal sealed class RemoveArrayValuesPatch<T, TValue> : IPatchAction<T>
        where T : class
    {
        private readonly Func<T, TValue[]> _getter;
        private readonly Action<T, TValue[]> _setter;
        private readonly TValue[] _values;

        public RemoveArrayValuesPatch(Func<T, TValue[]> getter, Action<T, TValue[]> setter, TValue[] values)
        {
            _getter = getter;
            _setter = setter;
            _values = values;
        }

        public void Apply(T instance)
        {
            var array = _getter(instance);
            if (array == null || array.Length == 0) return;

            var list = new List<TValue>();
            var values = _values;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < array.Length; i++)
            {
                var item = array[i];

                if (Array.IndexOf(values, item) != -1) continue;
                list.Add(item);
            }

            _setter(instance, list.ToArray());
        }
    }
}