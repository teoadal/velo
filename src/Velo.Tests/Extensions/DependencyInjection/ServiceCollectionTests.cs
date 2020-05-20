using Velo.DependencyInjection;
using Xunit;

namespace Velo.Tests.Extensions.DependencyInjection
{
    public abstract class ServiceCollectionTests : TestClass
    {
        public static TheoryData<DependencyLifetime> Lifetimes => new TheoryData<DependencyLifetime>
        {
            DependencyLifetime.Scoped,
            DependencyLifetime.Singleton,
            DependencyLifetime.Transient
        };
    }
}