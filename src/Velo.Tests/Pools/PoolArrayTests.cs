using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using Velo.Pools;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Pools
{
    public class PoolArrayTests : TestClass
    {
        private readonly PoolArray<int> _arrayPool;

        public PoolArrayTests(ITestOutputHelper output) : base(output)
        {
            _arrayPool = new PoolArray<int>();
        }

        [Fact]
        public void ClearArray()
        {
            var array = _arrayPool.Get(10);
            Array.Fill(array, int.MaxValue);
            _arrayPool.Return(array, true);
            foreach (var element in array)
            {
                Assert.Equal(0, element);
            }
        }
        
        [Fact]
        public void EmptyArray()
        {
            var array1 = _arrayPool.Get(0);
            var array2 = _arrayPool.Get(0);
            
            Assert.Same(array1, array2);
        }

        [Theory]
        [AutoData]
        public void GetArray(int arrayLength)
        {
            arrayLength = Math.Abs(arrayLength);

            for (var length = 1; length < arrayLength; length++)
            {
                var array = _arrayPool.Get(length);
                Assert.Equal(length, array.Length);
            }
        }

        [Theory]
        [AutoData]
        public void GetArray_MultiThreading(int arrayLength)
        {
            arrayLength = Math.Abs(arrayLength);

            Parallel.For(1, arrayLength, length =>
            {
                var array = _arrayPool.Get(length);
                Assert.Equal(length, array.Length);
            });
        }

        [Fact]
        public void Throw_ArrayLessZero()
        {
            Assert.Throws<InvalidOperationException>(() => _arrayPool.Get(-1));
        }
    }
}