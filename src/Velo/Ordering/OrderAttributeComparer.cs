using System.Collections.Generic;
using Velo.Utils;

namespace Velo.Ordering
{
    internal sealed class OrderAttributeComparer<T> : IComparer<T>
        where T : class
    {
        private readonly int _defaultValue;

        public OrderAttributeComparer(int defaultValue = 250)
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