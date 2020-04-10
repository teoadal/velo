using Velo.Collections;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections
{
    public class EmptyEnumeratorShould : TestClass
    {
        public EmptyEnumeratorShould(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void BeEmpty()
        {
            var instance = EmptyEnumerator<int>.Instance;
            
            Assert.Equal(default, instance.Current);
            Assert.False(instance.MoveNext());
        }
    }
}