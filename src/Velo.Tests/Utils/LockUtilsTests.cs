using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Utils
{
    public class LockUtilsTests : TestClass
    {
        private readonly object _lock;
        
        public LockUtilsTests(ITestOutputHelper output) : base(output)
        {
            _lock = new object();
        }

        [Fact]
        public void LockEnter()
        {
            var lockStruct = Lock.Enter(_lock);
            Assert.True(Monitor.IsEntered(_lock));
            
            lockStruct.Dispose();
            Assert.False(Monitor.IsEntered(_lock));
        }
        
        [Fact]
        public void LockUsing()
        {
            using (Lock.Enter(_lock))
            {
                Assert.True(Monitor.IsEntered(_lock));    
            }

            Assert.False(Monitor.IsEntered(_lock));
        }
    }
}