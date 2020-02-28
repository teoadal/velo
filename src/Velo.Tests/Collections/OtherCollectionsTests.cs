using Velo.Collections;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections
{
    public class OtherCollectionsTests : TestClass
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