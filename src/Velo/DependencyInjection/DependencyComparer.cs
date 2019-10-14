using System;
using System.Collections.Generic;
using Velo.DependencyInjection.Dependencies;
using Velo.Ordering;
using Velo.Utils;

namespace Velo.DependencyInjection
{
    internal sealed class DependencyComparer : IComparer<Dependency>
    {
        public static DependencyComparer Instance = new DependencyComparer();

        public static Dependency[] Sort(Dependency[] array)
        {
            Array.Sort(array, Instance);
            return array;
        }
        
        private DependencyComparer()
        {
        }

        public int Compare(Dependency first, Dependency second)
        {
            var firstOrder = GetOrder(first);
            var secondOrder = GetOrder(second);

            return firstOrder.CompareTo(secondOrder);
        }

        private int GetOrder(Dependency dependency)
        {
            var implementation = dependency.Resolver.Implementation;
            if (implementation == null) return OrderAttribute.DEFAULT_ORDER;

            return ReflectionUtils.TryGetAttribute<OrderAttribute>(implementation, out var attribute)
                ? attribute.Order
                : OrderAttribute.DEFAULT_ORDER;
        }
    }
}