using System;
using System.Collections.Generic;
using System.Reflection;
using Velo.Ordering;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo
{
    public class OrderingTests : TestBase
    {
        private readonly IComparer<IRepository> _comparer;
        private readonly IRepository[] _repositories;
        
        public OrderingTests(ITestOutputHelper output) : base(output)
        {
            _comparer = new OrderAttributeComparer<IRepository>();

            _repositories = new IRepository[]
            {
                new OtherFooRepository(null, null),    // 2
                new BooRepository(null, null),         // without order attribute
                new FooRepository(null, null),         // 1 
            };
        }

        [Fact]
        public void Order_Attribute_Exists()
        {
            var attribute = typeof(FooRepository).GetCustomAttribute<OrderAttribute>();
            
            Assert.NotNull(attribute);
            Assert.Equal(1, attribute.Order);
        }
        
        [Fact]
        public void Order_Array_Sort()
        {
            Array.Sort(_repositories, _comparer);

            Assert.Equal(1, _repositories[0].GetType().GetCustomAttribute<OrderAttribute>().Order);
            Assert.Equal(2, _repositories[1].GetType().GetCustomAttribute<OrderAttribute>().Order);
            Assert.Null(_repositories[2].GetType().GetCustomAttribute<OrderAttribute>());
        }
        
        [Fact]
        public void Order_Array_Sort_Without_OrderAttribute()
        {
            var instances = new object[] {new Boo(), new Foo(), new Configuration()};
            var comparer = new OrderAttributeComparer<object>();
            
            Array.Sort(instances, comparer);

            Assert.IsType<Boo>(instances[0]);
            Assert.IsType<Foo>(instances[1]);
            Assert.IsType<Configuration>(instances[2]);
        }
    }
}