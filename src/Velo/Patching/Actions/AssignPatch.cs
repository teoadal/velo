using System;

namespace Velo.Patching.Actions
{
    internal sealed class AssignPatch<T, TValue> : IPatchAction<T>
        where T : class
    {
        private readonly Action<T, TValue> _setter;
        private readonly TValue _value;

        public AssignPatch(Action<T, TValue> setter, TValue value)
        {
            _setter = setter;
            _value = value;
        }

        public void Apply(T instance)
        {
            _setter(instance, _value);
        }
    }
}