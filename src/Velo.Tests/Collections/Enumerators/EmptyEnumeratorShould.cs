using Velo.Collections.Enumerators;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Collections.Enumerators
{
    public class EmptyEnumeratorShould : TestClass
    {
        public EmptyEnumeratorShould(ITestOutputHelper output) : base(output)
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