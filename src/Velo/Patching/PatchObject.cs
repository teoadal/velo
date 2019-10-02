using System;
using System.Linq.Expressions;

namespace Velo.Patching
{
    internal sealed class PatchObject<T> : IPatchObject
    {
        public Action<T> GetInitializer<TValue>(Expression<Func<T, TValue>> path)
        {
            throw new NotImplementedException();
        }

        public Action<T> GetDecrement<TValue>(Expression<Func<T, TValue>> path)
        {
            throw new NotImplementedException();
        }
        
        public Action<T> GetIncrement<TValue>(Expression<Func<T, TValue>> path)
        {
            throw new NotImplementedException();
        }
        
        public Func<T, TValue> GetGetter<TValue>(Expression<Func<T, TValue>> path)
        {
            throw new NotImplementedException();
        }
        
        public Action<T, TValue> GetSetter<TValue>(Expression<Func<T, TValue>> path)
        {
            throw new NotImplementedException();
        }
    }

    internal interface IPatchObject
    {
    }
}