using System;
using System.Collections.Generic;

namespace Velo.Patching.CollectionActions
{
    internal sealed class AddValuesPatch<T, TCollection, TValue> : IPatchAction<T>
        where T : class
        where TCollection: ICollection<TValue>
    {
        private readonly Action<T> _initializer;
        private readonly Func<T, TCollection> _getter;
        private readonly TValue[] _values;

        public AddValuesPatch(Action<T> initializer, Func<T, TCollection> getter, TValue[] values)
        {
            _initializer = initializer;
            _getter = getter;
            _values = values;
        }

        public void Apply(T instance)
        {
            var collection = GetCollection(instance);
            var values = _values;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < values.Length; i++)
            {
                collection.Add(values[i]);
            }
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