using System;
using Velo.Collections;

namespace Velo.Patching.ArrayActions
{
    internal sealed class RemoveArrayValuePatch<T, TValue> : IPatchAction<T>
        where T : class
    {
        private readonly Func<T, TValue[]> _getter;
        private readonly Action<T, TValue[]> _setter;
        private readonly TValue _value;

        public RemoveArrayValuePatch(Func<T, TValue[]> getter, Action<T, TValue[]> setter, TValue value)
        {
            _getter = getter;
            _setter = setter;
            _value = value;
        }

        public void Apply(T instance)
        {
            var array = _getter(instance);
            if (array.NullOrEmpty()) return;

            var index = Array.IndexOf(array, _value);
            if (index == -1) return;
            if (array.Length == 1)
            {
                _setter(instance, Array.Empty<TValue>());
            }
            else
            {
                var newArray = new TValue[array.Length - 1];
                var newArrayIndex = 0;
                for (var i = 0; i < array.Length; i++)
                {
                    if (i == index) continue;
                    newArray[newArrayIndex++] = array[i];
                }

                _setter(instance, newArray);
            }
        }
    }
}