using System.Collections.Generic;
using FluentAssertions;
using Velo.DependencyInjection.Dependencies;
using Velo.DependencyInjection.Resolvers;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.DependencyInjection
{
    public class ScopedDependencyShould : TestClass
    {
        private readonly DependencyProvider _provider;

        public ScopedDependencyShould(ITestOutputHelper output) : base(output)
        {
            _provider = new DependencyCollection()
                .AddScoped(ctx => new BooRepository(null, null))
                .BuildProvider();
        }

        [Fact]
        public void HasValidContracts()
        {
            var implementation = typeof(BooRepository);
            var contracts = new[] {implementation};
            var resolver = new DelegateResolver(implementation, ctx => new BooRepository(null, null));
            var dependency = new ScopedDependency(contracts, resolver);

            dependency.Contracts.Should().Contain(contracts);
            ((IDependency) dependency).Contracts.Should().Contain(contracts);
        }
        
        [Fact]
        public void DisposeInternalCollection()
        {
            var repositories = new List<BooRepository> {_provider.GetRequiredService<BooRepository>()};

            var scope = _provider.CreateScope();
            repositories.Add(scope.GetService<BooRepository>());

            _provider.Dispose();

            repositories.Should().Contain(r => r.Disposed);
        }

        [Fact]
        public void NotDisposeInternalCollectionIfScopeNotClosed()
        {
            var repositories = new HashSet<BooRepository> {_provider.GetRequiredService<BooRepository>()};
            
            for (var i = 0; i < 100; i++)
            {
                var scope = _provider.CreateScope();
                repositories.Add(scope.GetService<BooRepository>()).Should().BeTrue();
            }

            repositories.Should().Contain(r => !r.Disposed);
        }
    }
}