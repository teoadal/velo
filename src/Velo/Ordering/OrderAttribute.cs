using System;

namespace Velo.Ordering
{
    public sealed class OrderAttribute: Attribute
    {
        public int Order { get; }

        public OrderAttribute(int order)
        {
            Order = order;
        }
    }
}