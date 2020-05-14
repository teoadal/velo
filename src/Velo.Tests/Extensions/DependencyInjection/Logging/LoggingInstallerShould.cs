using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Velo.Logging;
using Velo.Logging.Enrichers;
using Velo.Logging.Provider;
using Velo.Logging.Renderers;
using Velo.Logging.Writers;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Extensions.DependencyInjection.Logging
{
    public class LoggingInstallerShould : TestClass
    {
        private readonly IServiceCollection _services;

        public LoggingInstallerShould(ITestOutputHelper output) : base(output)
        {
            _services = new ServiceCollection()
                .AddLogging();
        }

        [Fact]
        public void AddEnricher()
        {
            _services.AddLogEnricher<TimeStampEnricher>();

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(ILogEnricher) &&
                descriptor.ImplementationType == typeof(TimeStampEnricher));
        }

        [Fact]
        public void AddWriter()
        {
            _services.AddLogWriter<DefaultConsoleWriter>();

            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(ILogWriter) &&
                descriptor.ImplementationType == typeof(DefaultConsoleWriter));
        }

        [Fact]
        public void InstallRenderersCollection()
        {
            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IRenderersCollection) &&
                descriptor.ImplementationType == typeof(RenderersCollection));
        }

        [Fact]
        public void InstallSingletonRenderersCollection()
        {
            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(IRenderersCollection) &&
                descriptor.Lifetime == ServiceLifetime.Singleton);
        }

        [Fact]
        public void InstallLogProvider()
        {
            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(ILogProvider));
        }

        [Fact]
        public void InstallScopedLogProvider()
        {
            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(ILogProvider) &&
                descriptor.Lifetime == ServiceLifetime.Scoped);
        }

        [Fact]
        public void InstallLogger()
        {
            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(ILogger<>) &&
                descriptor.ImplementationType == typeof(Logger<>));
        }

        [Fact]
        public void InstallScopedLogger()
        {
            _services.Should().Contain(descriptor =>
                descriptor.ServiceType == typeof(ILogger<>) &&
                descriptor.Lifetime == ServiceLifetime.Scoped);
        }

        [Fact]
        public void ResolveProvider()
        {
            _services
                .AddLogging()
                .BuildServiceProvider()
                .GetService<ILogProvider>()
                .Should().NotBeNull();
        }

        [Fact]
        public void ResolveNullProvider()
        {
            _services
                .BuildServiceProvider()
                .GetService<ILogProvider>()
                .Should().BeOfType<NullLogProvider>();
        }

        [Fact]
        public void ResolveLogger()
        {
            _services
                .AddLogging()
                .AddDefaultConsoleLogWriter()
                .BuildServiceProvider()
                .GetService<ILogger<Boo>>()
                .Should().NotBeNull();
        }
    }
}