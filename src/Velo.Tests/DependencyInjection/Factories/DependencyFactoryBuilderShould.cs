using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.DependencyInjection.Factories;
using Velo.Logging.Provider;
using Velo.Logging.Writers;
using Xunit;

namespace Velo.Tests.DependencyInjection.Factories
{
    public class DependencyFactoryBuilderShould : DITestClass
    {
        private readonly Type _contract;
        private readonly Type _implementation;

        private readonly Mock<IDependencyEngine> _engine;

        public DependencyFactoryBuilderShould()
        {
            _contract = typeof(ILogProvider);
            _implementation = typeof(LogProvider);
            
            _engine = new Mock<IDependencyEngine>();
        }

        [Fact]
        public void BuildFactory()
        {
            var factory = CreateBuilder().Build();
            factory.Should().NotBeNull();
        }
        
        #region Factory

        [Fact]
        public void Factory_Applicable()
        {
            var factory = CreateBuilder().Build();
            factory.Applicable(_contract).Should().BeTrue();
        }
        
        [Theory, MemberData(nameof(Lifetimes))]
        public void Factory_CalculateLifetime(DependencyLifetime lifetime)
        {
            var factory = CreateBuilder().DependedLifetime().Build();

            SetupRequiredDependencies(_engine, typeof(ILogWriter), lifetime);
            
            var dependency = factory.BuildDependency(_contract, _engine.Object);
            dependency.Lifetime.Should().Be(lifetime);
        }

        [Fact]
        public void Factory_CreateIfNoApplicableDependencies()
        {
            var factory = CreateBuilder()
                .CreateIf<NullLogProvider>(engine => !engine.Contains(typeof(ILogWriter)))
                .Build();

            var dependency = factory.BuildDependency(_contract, _engine.Object);
            dependency.Implementation.Should().Be(typeof(NullLogProvider));
        }
        
        [Theory, MemberData(nameof(Lifetimes))]
        public void Factory_UseLifetime(DependencyLifetime lifetime)
        {
            var factory = CreateBuilder()
                .Lifetime(lifetime)
                .Build();

            SetupApplicableDependencies(_engine, typeof(ILogWriter), DependencyLifetime.Transient);
            
            var dependency = factory.BuildDependency(_contract, _engine.Object);
            dependency.Lifetime.Should().Be(lifetime);
        }

        #endregion

        private DependencyFactoryBuilder CreateBuilder()
        {
            return new DependencyFactoryBuilder(_contract, _implementation);
        }
    }
}