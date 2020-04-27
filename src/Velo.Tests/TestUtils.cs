using System;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;

namespace Velo.Tests
{
    public static class TestUtils
    {
        public static Mock<IDependency> MockDependency(DependencyLifetime lifetime = DependencyLifetime.Singleton,
            Type contract = null)
        {
            var dependency = new Mock<IDependency>();
            dependency
                .SetupGet(d => d.Lifetime)
                .Returns(lifetime);

            if (contract != null)
            {
                dependency
                    .SetupGet(d => d.Contracts)
                    .Returns(new[] {contract});
            }

            return dependency;
        }

        public static Mock<IDependency> SetupResolverImplementation(this Mock<IDependency> dependency,
            Type implementation)
        {
            dependency
                .SetupGet(d => d.Resolver)
                .Returns(new Mock<DependencyResolver>(implementation).Object);

            return dependency;
        }

        public static Mock<IDependencyEngine> SetupApplicable(this Mock<IDependencyEngine> engine, Type contract,
            params IDependency[] result)
        {
            engine
                .Setup(e => e.GetApplicable(contract))
                .Returns(result);

            return engine;
        }

        public static Mock<IDependencyEngine> SetupRequired(this Mock<IDependencyEngine> engine, Type contract,
            IDependency result)
        {
            engine
                .Setup(e => e.GetRequiredDependency(contract))
                .Returns(result);

            return engine;
        }

        public static Mock<IDependencyEngine> MockDependencyEngine(Type contract, IDependency result)
        {
            var engine = new Mock<IDependencyEngine>();
            engine
                .Setup(e => e.GetRequiredDependency(contract))
                .Returns(result);

            return engine;
        }
    }
}