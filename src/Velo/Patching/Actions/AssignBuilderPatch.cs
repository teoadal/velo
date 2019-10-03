using System;

namespace Velo.Patching.Actions
{
    internal sealed class AssignBuilderPatch<T, TValue> : IPatchAction<T> where T : class
    {
        private readonly Func<T, TValue> _builder;
        private readonly Action<T, TValue> _setter;
        

        public AssignBuilderPatch(Action<T, TValue> setter, Func<T, TValue> builder)
        {
            _builder = builder;
            _setter = setter;
        }

        public void Apply(T instance)
        {
            var value = _builder(instance);
            _setter(instance, value);
        }
    }
}