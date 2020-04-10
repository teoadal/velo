using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Metrics.Counters;
using Velo.Metrics.Provider;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Metrics
{
    public class MetricsProviderFactoryShould : TestClass
    {
        private readonly Type _contract;
        private readonly Mock<IDependencyEngine> _dependencyEngine;
        private readonly MetricsProviderFactory _factory;

        public MetricsProviderFactoryShould(ITestOutputHelper output) : base(output)
        {
            _contract = typeof(IMetricsProvider);
            _factory = new MetricsProviderFactory();
            
            _dependencyEngine = new Mock<IDependencyEngine>();
        }

        [Fact]
        public void Applicable()
        {
            _factory.Applicable(_contract);
        }

        [Fact]
        public void CreateProviderDependency()
        {
            _dependencyEngine
                .Setup(engine => engine.Contains(It.Is<Type>(type => type == typeof(ICounter))))
                .Returns(true);

            var dependency = _factory.BuildDependency(_contract, _dependencyEngine.Object);
            dependency.Resolver.Implementation.Should().Be<MetricsProvider>();
        }
        
        [Fact]
        public void CreateNullProviderDependency()
        {
            _dependencyEngine
                .Setup(engine => engine.Contains(It.Is<Type>(type => type == typeof(ICounter))))
                .Returns(false);

            var dependency = _factory.BuildDependency(_contract, _dependencyEngine.Object);
            dependency.Resolver.Implementation.Should().Be<NullMetricsProvider>();
        }
        
        [Fact]
        public void NotApplicable()
        {
            _factory.Applicable(typeof(object));
        }
    }
}