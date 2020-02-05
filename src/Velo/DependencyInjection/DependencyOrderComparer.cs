using System.Collections.Generic;
using Velo.DependencyInjection.Dependencies;
using Velo.Ordering;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    public sealed class DependencyOrderComparer : Comparer<IDependency>
    {
        public new static readonly Comparer<IDependency> Default = new DependencyOrderComparer();

        private readonly int _defaultValue;

        public DependencyOrderComparer(int defaultValue = OrderAttribute.DEFAULT_ORDER)
        {
            _defaultValue = defaultValue;
        }

        public override int Compare(IDependency first, IDependency second)
        {
            var firstOrder = GetOrder(first);
            var secondOrder = GetOrder(second);

            return firstOrder.CompareTo(secondOrder);
        }

        private int GetOrder(IDependency dependency)
        {
            var implementationType = dependency?.Resolver?.Implementation;

            if (implementationType == null) return _defaultValue;

            return ReflectionUtils.TryGetAttribute<OrderAttribute>(implementationType, out var attribute)
                ? attribute.Order
                : _defaultValue;
        }
    }
}