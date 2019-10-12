using System;

namespace Velo.Ordering
{
    public sealed class OrderAttribute: Attribute
    {
        // ReSharper disable once InconsistentNaming
        public const int DEFAULT_ORDER = 250;
        
        public int Order { get; }

        public OrderAttribute(int order)
        {
            Order = order;
        }
    }
}