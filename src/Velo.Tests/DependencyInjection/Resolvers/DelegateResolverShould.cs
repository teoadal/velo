using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Xunit;

namespace Velo.Tests.DependencyInjection.Resolvers
{
    public class DelegateResolverShould : DITestClass
    {
        private readonly Type _contract;
        private readonly IBooRepository _instance;
        private readonly IServiceProvider _services;

        public DelegateResolverShould()
        {
            _contract = typeof(IBooRepository);
            _instance = Mock.Of<IBooRepository>();
            _services = Mock.Of<IServiceProvider>();
        }

        [Fact]
        public void ResolveInstance()
        {
            var builder = new Mock<Func<IServiceProvider, object>>();
            builder
                .Setup(b => b.Invoke(_services))
                .Returns(_instance);

            var resolver = new DelegateResolver(_contract, builder.Object);

            resolver
                .Invoking(r => r.Resolve(_contract, _services))
                .Should().NotThrow()
                .Which.Should().Be(_instance);
        }

        [Fact]
        public void ResolveParallel()
        {
            Parallel.For(0, 10, _ => ResolveInstance());
        }

        [Fact]
        public void ResolveTypedInstance()
        {
            var builder = new Mock<Func<IServiceProvider, IBooRepository>>();
            builder
                .Setup(b => b.Invoke(_services))
                .Returns(_instance);

            var resolver = new DelegateResolver<IBooRepository>(builder.Object);

            resolver
                .Invoking(r => r.Resolve(_contract, _services))
                .Should().NotThrow()
                .Which.Should().Be(_instance);
        }
    }
}