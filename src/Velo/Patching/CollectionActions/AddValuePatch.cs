using System;
using System.Collections.Generic;

namespace Velo.Patching.CollectionActions
{
    internal sealed class AddValuePatch<T, TCollection, TValue> : IPatchAction<T>
        where T : class
        where TCollection: ICollection<TValue>
    {
        private readonly Func<T, TCollection> _getter;
        private readonly Action<T> _initializer;
        private readonly TValue _value;

        public AddValuePatch(Action<T> initializer, Func<T, TCollection> getter, TValue value)
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