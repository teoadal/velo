using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Velo.Patching.Actions;

namespace Velo.Patching
{
    public sealed class Patch<T> where T : class
    {
        private readonly List<IPatchAction<T>> _actions;
        private readonly PatchObject<T> _patchObject;

        internal Patch(PatchObject<T> patchObject)
        {
            _patchObject = patchObject;

            _actions = new List<IPatchAction<T>>();
        }

        public Patch<T> AddValue<TValue>(Expression<Func<T, ICollection<TValue>>> collection, TValue value)
        {
            var initializer = _patchObject.GetInitializer(collection);
            var getter = _patchObject.GetGetter(collection);

            return Execute(new AddValuePatch<T, TValue>(initializer, getter, value));
        }

        public Patch<T> AddValues<TValue>(Expression<Func<T, ICollection<TValue>>> collection, params TValue[] values)
        {
            var initializer = _patchObject.GetInitializer(collection);
            var getter = _patchObject.GetGetter(collection);

            return Execute(new AddValuesPatch<T, TValue>(initializer, getter, values));
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
            var setter = _patchObject.GetSetter(path);
            return Execute(new AssignPatch<T, TValue>(setter, value));
        }

        public Patch<T> Assign<TValue>(Expression<Func<T, TValue>> path, Func<T, TValue> builder)
        {
            var setter = _patchObject.GetSetter(path);
            return Execute(new AssignBuilderPatch<T, TValue>(setter, builder));
        }

        public Patch<T> ClearValues<TValue>(Func<T, ICollection<TValue>> collection)
        {
            return Execute(new ClearValuesPatch<T, TValue>(collection));
        }

        public Patch<T> Decrement<TValue>(Expression<Func<T, TValue>> path)
        {
            var decrement = _patchObject.GetDecrement(path);
            return Execute(new DecrementPatch<T>(decrement));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Patch<T> Execute(IPatchAction<T> patch)
        {
            _actions.Add(patch);
            return this;
        }

        public Patch<T> Increment<TValue>(Expression<Func<T, TValue>> path)
        {
            var increment = _patchObject.GetIncrement(path);
            return Execute(new IncrementPatch<T, TValue>(increment));
        }

        public Patch<T> RemoveValue<TValue>(Func<T, ICollection<TValue>> collection, TValue value)
        {
            return Execute(new RemoveValuePatch<T, TValue>(collection, value));
        }

        public Patch<T> RemoveValues<TValue>(Func<T, ICollection<TValue>> collection, params TValue[] values)
        {
            return Execute(new RemoveValuesPatch<T, TValue>(collection, values));
        }

        public Patch<T> ReplaceValue<TValue>(Func<T, IList<TValue>> collection, TValue oldValue, TValue newValue)
        {
            return Execute(new ReplacePatch<T, TValue>(collection, oldValue, newValue));
        }
    }
}