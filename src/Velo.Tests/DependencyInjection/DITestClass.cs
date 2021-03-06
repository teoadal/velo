using System;
using Moq;
using Moq.Protected;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Factories;
using Velo.DependencyInjection.Resolvers;
using Xunit;

namespace Velo.Tests.DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public abstract class DITestClass : TestClass
    {
        protected static Mock<IDependencyFactory> MockDependencyFactory(Type contract)
        {
            var factory = new Mock<IDependencyFactory>();
            factory
                .Setup(f => f.Applicable(contract))
                .Returns(true);

            return factory;
        }
        
        protected static Mock<DependencyResolver> MockResolver(Type implementation,
            Func<Type, IServiceProvider, object> resolveInstance)
        {
            var resolver = new Mock<DependencyResolver>(implementation);

            resolver
                .Protected()
                .Setup<object>("ResolveInstance", implementation, ItExpr.IsAny<IServiceProvider>())
                .Returns(resolveInstance);

            return resolver;
        }

        public static TheoryData<DependencyLifetime> Lifetimes => new TheoryData<DependencyLifetime>
        {
            DependencyLifetime.Scoped,
            DependencyLifetime.Singleton,
            DependencyLifetime.Transient
        };
    }
}