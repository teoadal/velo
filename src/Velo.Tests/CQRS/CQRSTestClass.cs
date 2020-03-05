using System.Collections.Generic;
using Velo.DependencyInjection;
using Xunit.Abstractions;

namespace Velo.Tests.CQRS
{
    // ReSharper disable once InconsistentNaming
    public abstract class CQRSTestClass : TestClass
    {
        protected CQRSTestClass(ITestOutputHelper output) : base(output)
        {
        }
        
        public static IEnumerable<object[]> Lifetimes
        {
            // ReSharper disable once UnusedMember.Global
            get
            {
                return new[]
                {
                    new object[] {DependencyLifetime.Scoped},
                    new object[] {DependencyLifetime.Singleton},
                    new object[] {DependencyLifetime.Transient}
                };
            }
        }
    }
}