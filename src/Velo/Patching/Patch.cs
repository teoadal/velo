using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Velo.Patching.ArrayActions;
using Velo.Patching.CollectionActions;
using Velo.Patching.CommonActions;
using Velo.Patching.Methods;
using Velo.Patching.NumberActions;

namespace Velo.Patching
{
    public sealed class Patch<T> where T : class
    {
        private readonly List<IPatchAction<T>> _actions;
        private readonly PatchMethods<T> _patchMethods;

        internal Patch(PatchMethods<T> patchMethods)
        {
            _patchMethods = patchMethods;

            _actions = new List<IPatchAction<T>>();
        }

        public Patch<T> AddValue<TCollection, TValue>(Expression<Func<T, TCollection>> collection, TValue value)
            where TCollection : ICollection<TValue>
        {
            var methods = _patchMethods.GetCommonMethods(collection);
            return Execute(new AddValuePatch<T, TCollection, TValue>(methods.Initializer, methods.Getter, value));
        }

        public Patch<T> AddValue<TValue>(Expression<Func<T, TValue[]>> array, TValue value)
        {
            var methods = _patchMethods.GetCommonMethods(array);
            return Execute(new AddArrayValuePatch<T, TValue>(methods.Getter, methods.Setter, value));
        }

        public Patch<T> AddValues<TCollection, TValue>(Expression<Func<T, TCollection>> array,
            params TValue[] values)
            where TCollection : ICollection<TValue>
        {
            var methods = _patchMethods.GetCommonMethods(array);
            return Execute(new AddValuesPatch<T, TCollection, TValue>(methods.Initializer, methods.Getter, values));
        }

        public Patch<T> AddValues<TValue>(Expression<Func<T, TValue[]>> collection, params TValue[] values)
        {
            var methods = _patchMethods.GetCommonMethods(collection);
            return Execute(new AddArrayValuesPatch<T, TValue>(methods.Getter, methods.Setter, values));
        }

        public void Apply(T instance)
        {
            foreach (var patch in _actions)
            {
                patch.Apply(instance);
            }
        }

        public Patch<T> Assign<TValue>(Expression<Func<T, TValue>> path, TValue value)
        {
            var setter = _patchMethods.GetSetter(path);
            return Execute(new AssignPatch<T, TValue>(setter, value));
        }

        public Patch<T> Assign<TValue>(Expression<Func<T, TValue>> path, Func<T, TValue> builder)
        {
            var setter = _patchMethods.GetSetter(path);
            return Execute(new AssignBuilderPatch<T, TValue>(setter, builder));
        }

        public Patch<T> ClearValues<TValue>(Func<T, ICollection<TValue>> collection)
        {
            return Execute(new ClearValuesPatch<T, TValue>(collection));
        }

        public Patch<T> ClearValues<TValue>(Expression<Func<T, TValue[]>> array)
        {
            var methods = _patchMethods.GetCommonMethods(array);
            return Execute(new ClearArrayValuesPath<T, TValue>(methods.Getter, methods.Setter));
        }
        
        public Patch<T> Decrement<TValue>(Expression<Func<T, TValue>> path)
        {
            var decrement = _patchMethods.GetDecrement(path);
            return Execute(new DecrementPatch<T>(decrement));
        }

        public Patch<T> Drop<TValue>(Expression<Func<T, TValue>> path)
        {
            var setter = _patchMethods.GetSetter(path);
            return Execute(new AssignPatch<T, TValue>(setter, default));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Patch<T> Execute(IPatchAction<T> patch)
        {
            _actions.Add(patch);
            return this;
        }

        public Patch<T> Increment<TValue>(Expression<Func<T, TValue>> path)
        {
            var increment = _patchMethods.GetIncrement(path);
            return Execute(new IncrementPatch<T>(increment));
        }

        public Patch<T> RemoveValue<TValue>(Func<T, ICollection<TValue>> collection, TValue value)
        {
            return Execute(new RemoveValuePatch<T, TValue>(collection, value));
        }

        public Patch<T> RemoveValue<TValue>(Expression<Func<T, TValue[]>> array, TValue value)
        {
            var methods = _patchMethods.GetCommonMethods(array);
            return Execute(new RemoveArrayValuePatch<T, TValue>(methods.Getter, methods.Setter, value));
        }
        
        public Patch<T> RemoveValues<TValue>(Func<T, ICollection<TValue>> collection, params TValue[] values)
        {
            return Execute(new RemoveValuesPatch<T, TValue>(collection, values));
        }

        public Patch<T> RemoveValues<TValue>(Expression<Func<T, TValue[]>> array, params TValue[] values)
        {
            var methods = _patchMethods.GetCommonMethods(array);
            return Execute(new RemoveArrayValuesPatch<T, TValue>(methods.Getter, methods.Setter, values));
        }
        
        public Patch<T> ReplaceValue<TValue>(Func<T, IList<TValue>> collection, TValue oldValue, TValue newValue)
        {
            return Execute(new ReplacePatch<T, TValue>(collection, oldValue, newValue));
        }
    }
}