using Velo.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Extensions.DependencyInjection
{
    public abstract class ServiceCollectionTests : TestClass
    {
        protected ServiceCollectionTests(ITestOutputHelper output) : base(output)
        {
        }

        public static TheoryData<DependencyLifetime> Lifetimes => new TheoryData<DependencyLifetime>
        {
            DependencyLifetime.Scoped,
            DependencyLifetime.Singleton,
            DependencyLifetime.Transient
        };
    }
}