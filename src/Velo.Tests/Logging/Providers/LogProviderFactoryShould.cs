using System;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Logging.Provider;
using Velo.Logging.Writers;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Logging.Providers
{
    public class LogProviderFactoryShould : TestClass
    {
        private readonly Type _contract;
        private readonly Mock<IDependencyEngine> _dependencyEngine;
        private readonly LogProviderFactory _factory;

        public LogProviderFactoryShould(ITestOutputHelper output) : base(output)
        {
            _contract = typeof(ILogProvider);
            _factory = new LogProviderFactory();
            
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
                .Setup(engine => engine.Contains(It.Is<Type>(type => type == typeof(ILogWriter))))
                .Returns(true);

            var dependency = _factory.BuildDependency(_contract, _dependencyEngine.Object);
            dependency.Resolver.Implementation.Should().Be<LogProvider>();
        }
        
        [Fact]
        public void CreateNullProviderDependency()
        {
            _dependencyEngine
                .Setup(engine => engine.Contains(It.Is<Type>(type => type == typeof(ILogWriter))))
                .Returns(false);

            var dependency = _factory.BuildDependency(_contract, _dependencyEngine.Object);
            dependency.Resolver.Implementation.Should().Be<NullLogProvider>();
        }
        
        [Fact]
        public void NotApplicable()
        {
            _factory.Applicable(typeof(object));
        }
    }
}