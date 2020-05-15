using System.Linq;
using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Collections.Local;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections.Local
{
    public class LocalListExtensionsShould : TestClass
    {
        public LocalListExtensionsShould(ITestOutputHelper output) : base(output)
        {
        }

        [Theory, AutoData]
        public void SumIntValues(int[] numbers)
        {
            var localList = new LocalList<int>(numbers);
            localList.Sum().Should().Be(numbers.Sum());
        }
    }
}