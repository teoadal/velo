using System;

namespace Velo.Patching.Actions
{
    internal sealed class DecrementPatch<T> : IPatchAction<T>
        where T : class
    {
        private readonly Action<T> _decrement;

        public DecrementPatch(Action<T> decrement)
        {
            _decrement = decrement;
        }

        public void Apply(T instance)
        {
            _decrement(instance);
        }
    }
}