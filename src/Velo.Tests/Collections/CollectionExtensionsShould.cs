using System;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Collections;
using Xunit;

namespace Velo.Tests.Collections
{
    public class CollectionExtensionsShould : TestClass
    {
        [Fact]
        public void Contains()
        {
            var array = Fixture.CreateMany<int>().ToArray();
            array.Contains((element, i) => element == i, array.Max());
        }
        
        [Fact]
        public void CatchNullArray()
        {
            int[] array = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            array.NullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void CatchEmptyArray()
        {
            var array = Array.Empty<int>();
            array.NullOrEmpty().Should().BeTrue();
        }

        [Fact]
        public void CatchNotNullOrEmptyArray()
        {
            int[] array = {1};
            array.NullOrEmpty().Should().BeFalse();
        }

        [Theory]
        [AutoData]
        public void ExecuteForeach(int[] values)
        {
            var counter = 0;
            values.Foreach(item => item.Should().Be(values[counter++]));
        }

        [Theory]
        [AutoData]
        public void ExecuteDo(int[] values)
        {
            var counter = 0;
            foreach (var item in values.Do(v => { }))
            {
                item.Should().Be(values[counter++]);
            }
        }
        
        [Fact]
        public void NotContains()
        {
            var array = Fixture.CreateMany<int>().ToArray();
            array.Contains((element, i) => element == i, int.MinValue);
        }

    }
}