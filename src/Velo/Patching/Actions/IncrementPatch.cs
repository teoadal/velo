using System;

namespace Velo.Patching.Actions
{
    internal sealed class IncrementPatch<T, TValue> : IPatchAction<T>
        where T : class
    {
        private readonly Action<T> _increment;

        public IncrementPatch(Action<T> increment)
        {
            _increment = increment;
        }

        public void Apply(T instance)
        {
            _increment(instance);
        }
    }
}