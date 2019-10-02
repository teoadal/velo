using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Velo.Utils;

namespace Velo.Patching
{
    internal sealed class PatchObject<T> : IPatchObject
    {
        private readonly Func<PropertyInfo, NumberMethods> _buildNumberMethods;
        private readonly Dictionary<PropertyInfo, CommonMethods> _commonMethods;
        private readonly ConcurrentDictionary<PropertyInfo, NumberMethods> _numberMethods;
        private readonly Type _type;

        public PatchObject()
        {
            _buildNumberMethods = BuildNumberMethods;
            _type = Typeof<T>.Raw;

            var properties = _type.GetProperties();
            _commonMethods = new Dictionary<PropertyInfo, CommonMethods>(properties.Length);
            _numberMethods = new ConcurrentDictionary<PropertyInfo, NumberMethods>();

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (property.CanWrite)
                {
                    _commonMethods.Add(property, BuildCommonMethods(property));
                }
            }
        }

        public Action<T> GetInitializer<TValue>(Expression<Func<T, TValue>> path)
        {
            var member = (MemberExpression) path.Body;
            var propertyInfo = (PropertyInfo) member.Member;

            return (Action<T>) _commonMethods[propertyInfo].Initializer;
        }

        public Action<T> GetDecrement<TValue>(Expression<Func<T, TValue>> path)
        {
            var member = (MemberExpression) path.Body;
            var propertyInfo = (PropertyInfo) member.Member;

            var numberMethod = _numberMethods.GetOrAdd(propertyInfo, _buildNumberMethods);
            return (Action<T>) numberMethod.Decrement;
        }

        public Action<T> GetIncrement<TValue>(Expression<Func<T, TValue>> path)
        {
            var member = (MemberExpression) path.Body;
            var propertyInfo = (PropertyInfo) member.Member;

            var numberMethod = _numberMethods.GetOrAdd(propertyInfo, _buildNumberMethods);
            return (Action<T>) numberMethod.Increment;
        }

        public Func<T, TValue> GetGetter<TValue>(Expression<Func<T, TValue>> path)
        {
            var member = (MemberExpression) path.Body;
            var propertyInfo = (PropertyInfo) member.Member;

            return (Func<T, TValue>) _commonMethods[propertyInfo].Getter;
        }

        public Action<T, TValue> GetSetter<TValue>(Expression<Func<T, TValue>> path)
        {
            var member = (MemberExpression) path.Body;
            var propertyInfo = (PropertyInfo) member.Member;

            return (Action<T, TValue>) _commonMethods[propertyInfo].Setter;
        }

        private CommonMethods BuildCommonMethods(PropertyInfo property)
        {
            return new CommonMethods(
                ExpressionUtils.BuildInitializer(_type, property),
                ExpressionUtils.BuildGetter(_type, property),
                ExpressionUtils.BuildSetter(_type, property));
        }

        private NumberMethods BuildNumberMethods(PropertyInfo property)
        {
            return new NumberMethods(
                ExpressionUtils.BuildDecrement(_type, property),
                ExpressionUtils.BuildIncrement(_type, property));
        }

        private readonly struct CommonMethods
        {
            public readonly Delegate Getter;
            public readonly Delegate Initializer;
            public readonly Delegate Setter;

            public CommonMethods(Delegate initializer, Delegate getter, Delegate setter)
            {
                Initializer = initializer;
                Getter = getter;
                Setter = setter;
            }
        }

        private readonly struct NumberMethods
        {
            public readonly Delegate Decrement;
            public readonly Delegate Increment;

            public NumberMethods(Delegate decrement, Delegate increment)
            {
                Decrement = decrement;
                Increment = increment;
            }
        }
    }

    internal interface IPatchObject
    {
    }
}