using FluentAssertions;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection.Dependencies
{
    public class TransientDependencyShould : TestClass
    {
        private readonly DependencyProvider _provider;
        
        public TransientDependencyShould(ITestOutputHelper output) : base(output)
        {
            _provider = new DependencyCollection()
                .AddTransient(ctx => new BooRepository(null, null))
                .BuildProvider();
        }

        [Fact]
        public void HasValidContracts()
        {
            var implementation = typeof(BooRepository);
            var contracts = new[] {implementation};
            var resolver = new DelegateResolver(implementation, ctx => new BooRepository(null, null));
            var dependency = new TransientDependency(contracts, resolver);

            dependency.Contracts.Should().Contain(contracts);
            ((IDependency) dependency).Contracts.Should().Contain(contracts);
        }
        
        [Fact]
        public void NotDisposed()
        {
            var instance = _provider.GetRequiredService<BooRepository>();
            _provider.Dispose();

            instance.Disposed.Should().BeFalse();
        }
    }
}