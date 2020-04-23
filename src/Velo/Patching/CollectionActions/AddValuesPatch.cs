using System;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Patching.CollectionActions
{
    internal sealed class AddValuesPatch<T, TCollection, TValue> : IPatchAction<T>
        where T : class
        where TCollection: ICollection<TValue>
    {
        private readonly Action<T>? _initializer;
        private readonly Func<T, TCollection> _getter;
        private readonly TValue[] _values;

        public AddValuesPatch(Action<T>? initializer, Func<T, TCollection> getter, TValue[] values)
        {
            _initializer = initializer;
            _getter = getter;
            _values = values;
        }

        public void Apply(T instance)
        {
            var collection = GetCollection(instance);
            
            foreach (var value in _values)
            {
                collection.Add(value);
            }
        }

        private ICollection<TValue> GetCollection(T instance)
        {
            var collection = _getter(instance);
            
            if (collection != null) return collection;

            if (_initializer == null)
            {
                throw Error.InvalidOperation("Initializer isn't presented");
            }
            
            _initializer(instance);
            collection = _getter(instance);

            return collection;
        }
    }
}