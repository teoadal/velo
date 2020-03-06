using System;
using System.Collections.Generic;
using Moq;
using Moq.Protected;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Resolvers;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public abstract class DITestClass : TestClass
    {
        protected DITestClass(ITestOutputHelper output) : base(output)
        {
        }

        protected static Mock<IDependencyScope> MockScope()
        {
            var scope = new Mock<IDependencyScope>();

            scope.SetupAdd(s => s.Destroy += It.IsAny<Action<IDependencyScope>>());
            scope.SetupRemove(s => s.Destroy -= It.IsAny<Action<IDependencyScope>>());

            return scope;
        }

        protected static Mock<DependencyResolver> MockResolver(Type implementation,
            Func<Type, IDependencyScope, object> resolveInstance)
        {
            var resolver = new Mock<DependencyResolver>(implementation);

            resolver
                .Protected()
                .Setup<object>("ResolveInstance", implementation, ItExpr.IsAny<IDependencyScope>())
                .Returns(resolveInstance);

            return resolver;
        }

        public static IEnumerable<object[]> Lifetimes => new[]
        {
            new object[] {DependencyLifetime.Scoped},
            new object[] {DependencyLifetime.Singleton},
            new object[] {DependencyLifetime.Transient}
        };
    }
}