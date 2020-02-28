using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Velo.DependencyInjection;
using Velo.Pools;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Pools
{
    public class PoolsTests : TestClass
    {
        public PoolsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, AutoData]
        public void GetReturn_Array(int capacity)
        {
            capacity = Math.Abs(capacity);

            var pool = new Pool<int[]>(capacity, () => new int[0]);

            for (var i = 0; i < capacity; i++)
            {
                var array = pool.Get();
                Assert.NotNull(array);
                Assert.True(pool.Return(array));
            }
        }

        [Theory, AutoData]
        public void GetReturn_Object(int capacity)
        {
            capacity = Math.Abs(capacity);

            var pool = new Pool<Boo>(capacity, () => new Boo());

            for (var i = 0; i < capacity; i++)
            {
                var boo = pool.Get();
                Assert.NotNull(boo);
                Assert.True(pool.Return(boo));
            }
        }

        [Theory, AutoData]
        public void GetReturn_Array_MultiThreading(int capacity)
        {
            capacity = Math.Abs(capacity);

            var pool = new Pool<int[]>(capacity, () => new int[0]);

            Parallel.For(0, capacity, i =>
            {
                var array = pool.Get();
                Assert.NotNull(array);
                pool.Return(array);
            });
        }

        [Fact]
        public void ReturnFalse_IfFull()
        {
            var pool = new Pool<Boo>(10, () => new Boo());
            Assert.False(pool.Return(new Boo()));
        }
        
        [Fact]
        public void ReturnTrue_IfNotFull()
        {
            var pool = new Pool<Boo>(10, () => new Boo());
            pool.Get();
            Assert.True(pool.Return(new Boo()));
        }
        
        [Fact]
        public void Resolve_FromDependencyProvider()
        {
            var provider = new DependencyCollection()
                .AddPool(() => new Boo())
                .BuildProvider();

            var pool = provider.GetRequiredService<IPool<Boo>>();

            Assert.NotNull(pool);
            Assert.IsType<Pool<Boo>>(pool);
        }
    }
}