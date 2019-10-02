using System;
using System.Linq.Expressions;

namespace Velo.Utils
{
    internal static class ExpressionUtils
    {
        public static Action<T> BuildDecrement<T, TValue>(Expression<Func<T, TValue>> path)
        {
            throw new NotImplementedException();
        }

        public static Func<T, TValue> BuildGetter<T, TValue>(Expression<Func<T, TValue>> path)
        {
            throw new NotImplementedException();
        }

        public static Action<T> BuildIncrement<T, TValue>(Expression<Func<T, TValue>> path)
        {
            throw new NotImplementedException();
        }

        public static Action<T> BuildInitializer<T, TValue>(Expression<Func<T, TValue>> path)
        {
            throw new NotImplementedException();
        }

        public static Action<T, TValue> BuildSetter<T, TValue>(Expression<Func<T, TValue>> path)
        {
            throw new NotImplementedException();
        }
    }
}