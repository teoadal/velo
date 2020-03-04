using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection.Resolvers
{
    public class DelegateResolverShould : DITestClass
    {
        private readonly Type _contract;
        private readonly IDependencyScope _scope;

        public DelegateResolverShould(ITestOutputHelper output) : base(output)
        {
            _contract = typeof(IBooRepository);
            _scope = MockScope().Object;
        }

        [Fact]
        public void ResolveInstance()
        {
            var instance = new Mock<IBooRepository>();
            var builder = new Mock<Func<IServiceProvider, object>>();
            builder
                .Setup(b => b.Invoke(_scope))
                .Returns(instance.Object);

            var resolver = new DelegateResolver(_contract, builder.Object);

            resolver.Resolve(_contract, _scope).Should().Be(instance.Object);
        }

        [Fact]
        public void ResolveTypedInstance()
        {
            var instance = new BooRepository(null, null);
            var builder = new Mock<Func<IDependencyScope, BooRepository>>();
            builder
                .Setup(b => b.Invoke(_scope))
                .Returns(instance);

            var resolver = new DelegateResolver<BooRepository>(builder.Object);

            resolver.Resolve(_contract, _scope).Should().Be(instance);
        }
    }
}