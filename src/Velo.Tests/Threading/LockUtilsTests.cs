using System.Threading;
using Velo.Threading;
using Xunit;

namespace Velo.Tests.Threading
{
    public class LockUtilsTests : TestClass
    {
        private readonly object _lock;
        
        public LockUtilsTests()
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