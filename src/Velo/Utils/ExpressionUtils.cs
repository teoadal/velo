using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Velo.Utils
{
    public static class ExpressionUtils
    {
        private static readonly Type VoidType = typeof(void);
        private static readonly Expression VoidResult = Expression.Default(VoidType);

        #region Builders

        public static Func<T> BuildActivator<T>(
            ConstructorInfo? constructor = null,
            bool throwIfEmptyConstructorNotFound = true)
        {
            var type = typeof(T);

            constructor ??= ReflectionUtils.GetEmptyConstructor(type);

            if (constructor != null)
            {
                return Expression.Lambda<Func<T>>(Expression.New(constructor)).Compile();
            }
            else if (type.IsValueType)
            {
                return Expression.Lambda<Func<T>>(Expression.New(type)).Compile();
            }

            var exception = Error.DefaultConstructorNotFound(type);
            if (throwIfEmptyConstructorNotFound)
            {
                throw exception;
            }

            return Expression.Lambda<Func<T>>(Expression.Block(
                Expression.Throw(Expression.Constant(exception)),
                Expression.Default(type))).Compile();
        }

        public static Delegate BuildGetter(Type owner, PropertyInfo propertyInfo)
        {
            var instance = Expression.Parameter(owner, "instance");

            var body = Expression.Property(instance, propertyInfo);
            return Expression.Lambda(body, instance).Compile();
        }

        public static Delegate BuildSetter(Type owner, PropertyInfo propertyInfo)
        {
            var instance = Expression.Parameter(owner, "instance");
            var value = Expression.Parameter(propertyInfo.PropertyType, "value");
            var property = Expression.Property(instance, propertyInfo);

            var body = Expression.Block(
                Expression.Assign(property, value),
                VoidResult);

            return Expression.Lambda(body, instance, value).Compile();
        }

        #endregion

        public static MethodCallExpression Call(Expression instance, string methodName, Expression arg1)
        {
            var method = instance.Type.GetMethod(methodName);
            if (method == null) throw MethodNotFound(instance.Type, methodName);

            return Expression.Call(instance, method, arg1);
        }

        public static MethodCallExpression Call(Expression instance, string methodName, Expression arg1,
            Expression arg2)
        {
            var method = instance.Type.GetMethod(methodName);
            if (method == null) throw MethodNotFound(instance.Type, methodName);

            return Expression.Call(instance, method, arg1, arg2);
        }

        public static BinaryExpression SetProperty(Expression instance, PropertyInfo property, Expression propertyValue)
        {
            return Expression.Assign(Expression.Property(instance, property), propertyValue);
        }

        private static Exception MethodNotFound(Type owner, string methodName)
        {
            return Error.NotFound($"Method '{methodName}' not found in type '{ReflectionUtils.GetName(owner)}'");
        }
    }
}