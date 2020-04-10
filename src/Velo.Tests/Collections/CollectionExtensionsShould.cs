using System;
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
    }
}