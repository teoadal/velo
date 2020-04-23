using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Collections;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections
{
    public class CollectionExtensionsShould : TestClass
    {
        public CollectionExtensionsShould(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void CatchNullArray()
        {
            int[] array = null;
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
        
        [Theory, AutoData]
        public void ExecuteForeach(int[] values)
        {
            var counter = 0;
            values.Foreach(item => item.Should().Be(values[counter++]));
        }
        
        [Theory, AutoData]
        public void ExecuteDo(int[] values)
        {
            var counter = 0;
            foreach (var item in values.Do(v => { }))
            {
                item.Should().Be(values[counter++]);
            }
        }

    }
}