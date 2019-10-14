using System;
using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Ordering
{
    internal sealed class OrderAttributeComparer<T> : IComparer<T>
        where T : class
    {
        public static T[] Sort(T[] array, int defaultValue = OrderAttribute.DEFAULT_ORDER)
        {
            Array.Sort(array, new OrderAttributeComparer<T>(defaultValue));
            return array;
        }

        private readonly int _defaultValue;

        public OrderAttributeComparer(int defaultValue = OrderAttribute.DEFAULT_ORDER)
        {
            _defaultValue = defaultValue;
        }

        public int Compare(T first, T second)
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