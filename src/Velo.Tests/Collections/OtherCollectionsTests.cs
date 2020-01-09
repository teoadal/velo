using Xunit;
using Xunit.Abstractions;

namespace Velo.Collections
{
    public class OtherCollectionsTests : TestBase
    {
        public OtherCollectionsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Empty()
        {
            var instance = EmptyEnumerator<int>.Instance;
            
            Assert.Equal(default, instance.Current);
            Assert.False(instance.MoveNext());
        }
    }
}