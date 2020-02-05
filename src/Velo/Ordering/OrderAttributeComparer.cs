using System;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Ordering
{
    internal sealed class OrderAttributeComparer<T> : Comparer<T>
        where T : class
    {
        public new static readonly Comparer<T> Default = new OrderAttributeComparer<T>();
        
        public static T[] Sort(T[] array, int defaultValue = OrderAttribute.DEFAULT_ORDER)
        {
            Array.Sort(array, defaultValue == OrderAttribute.DEFAULT_ORDER
                ? Default
                : new OrderAttributeComparer<T>(defaultValue));
            return array;
        }

        private readonly int _defaultValue;

        public OrderAttributeComparer(int defaultValue = OrderAttribute.DEFAULT_ORDER)
        {
            _defaultValue = defaultValue;
        }

        public override int Compare(T first, T second)
        {
            var firstOrder = GetOrder(first);
            var secondOrder = GetOrder(second);

            return firstOrder.CompareTo(secondOrder);
        }

        private int GetOrder(T instance)
        {
            if (instance == null) return _defaultValue;

            return ReflectionUtils.TryGetAttribute<OrderAttribute>(instance.GetType(), out var attribute)
                ? attribute.Order
                : _defaultValue;
        }
    }
}