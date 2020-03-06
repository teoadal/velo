using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Velo.Tests.Extensions.DependencyInjection
{
    public abstract class ServiceCollectionTests : TestClass
    {
        protected ServiceCollectionTests(ITestOutputHelper output) : base(output)
        {
        }

        public static IEnumerable<object[]> Lifetimes => new[]
        {
            new object[] {ServiceLifetime.Scoped},
            new object[] {ServiceLifetime.Singleton},
            new object[] {ServiceLifetime.Transient},
        };
    }
}