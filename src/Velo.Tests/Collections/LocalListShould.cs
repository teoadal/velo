using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Collections
{
    public class LocalListShould : TestClass
    {
        public LocalListShould(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void InitializeCollection()
        {
            var list2 = new LocalList<int>(1, 2);
            list2.Contains(1).Should().BeTrue();
            list2.Contains(2).Should().BeTrue();
            list2.Length.Should().Be(2);
            
            var list3 = new LocalList<int>(1, 2, 3);
            list3.Contains(1).Should().BeTrue();
            list3.Contains(2).Should().BeTrue();
            list3.Contains(3).Should().BeTrue();
            list3.Length.Should().Be(3);
            
            var list4 = new LocalList<int>(1, 2, 3, 4);
            list4.Contains(1).Should().BeTrue();
            list4.Contains(2).Should().BeTrue();
            list4.Contains(3).Should().BeTrue();
            list4.Contains(4).Should().BeTrue();
            list4.Length.Should().Be(4);
            
            var list5 = new LocalList<int>(1, 2, 3, 4, 5);
            list5.Contains(1).Should().BeTrue();
            list5.Contains(2).Should().BeTrue();
            list5.Contains(3).Should().BeTrue();
            list5.Contains(4).Should().BeTrue();
            list5.Contains(5).Should().BeTrue();
            list5.Length.Should().Be(5);
        }
    }
}