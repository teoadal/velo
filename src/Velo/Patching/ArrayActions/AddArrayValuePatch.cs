using System;

namespace Velo.Patching.ArrayActions
{
    internal sealed class AddArrayValuePatch<T, TValue>: IPatchAction<T> where T: class
    {
        private readonly Func<T, TValue[]> _getter;
        private readonly Action<T, TValue[]> _setter;
        private readonly TValue _value;

        public AddArrayValuePatch(Func<T, TValue[]> getter, Action<T, TValue[]> setter, TValue value)
        {
            _getter = getter;
            _setter = setter;
            _value = value;
        }

        public void Apply(T instance)
        {
            var array = _getter(instance);
            
            if (array == null)
            {
                _setter(instance, new[] {_value});
            }
            else
            {
                var oldSize = array.Length;
                Array.Resize(ref array, oldSize + 1);
                array[oldSize] = _value;
                
                _setter(instance, array);
            }
        }
    }
}