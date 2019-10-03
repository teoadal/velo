using System;

namespace Velo.Patching.ArrayActions
{
    internal sealed class AddArrayValuesPatch<T, TValue>: IPatchAction<T> where T: class
    {
        private readonly Func<T, TValue[]> _getter;
        private readonly Action<T, TValue[]> _setter;
        private readonly TValue[] _values;

        public AddArrayValuesPatch(Func<T, TValue[]> getter, Action<T, TValue[]> setter, TValue[] values)
        {
            _getter = getter;
            _setter = setter;
            _values = values;
        }

        public void Apply(T instance)
        {
            var array = _getter(instance);
            
            if (array == null)
            {
                _setter(instance, _values);
            }
            else
            {
                var oldSize = array.Length;
                Array.Resize(ref array, oldSize + _values.Length);

                for (var i = 0; i < _values.Length; i++)
                {
                    array[oldSize + i] = _values[i];
                }
                
                _setter(instance, array);
            }
        }
    }
}