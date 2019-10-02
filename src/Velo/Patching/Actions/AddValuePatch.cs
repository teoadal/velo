using System;
using System.Collections.Generic;

namespace Velo.Patching.Actions
{
    internal sealed class AddValuePatch<T, TValue> : IPatchAction<T>
        where T : class
    {
        private readonly Func<T, ICollection<TValue>> _getter;
        private readonly Action<T> _initializer;
        private readonly TValue _value;

        public AddValuePatch(Action<T> initializer, Func<T, ICollection<TValue>> getter, TValue value)
        {
            _getter = getter;
            _initializer = initializer;
            _value = value;
        }

        public void Apply(T instance)
        {
            var collectionInstance = GetCollection(instance);
            collectionInstance.Add(_value);
        }

        private ICollection<TValue> GetCollection(T instance)
        {
            var collection = _getter(instance);

            if (collection != null) return collection;

            _initializer(instance);
            collection = _getter(instance);

            return collection;
        }
    }
}