using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Logging;
using Velo.Logging.Enrichers;
using Velo.Logging.Provider;
using Velo.Logging.Writers;
using Velo.Serialization.Models;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Logging
{
    public sealed class LoggingInstallerShould : TestClass
    {
        private readonly DependencyCollection _dependencies;

        public LoggingInstallerShould(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection()
                .AddLogWriter(new Mock<ILogWriter>().Object);
        }

        [Fact]
        public void AddDefaultConsoleWriter()
        {
            _dependencies.AddDefaultConsoleLogWriter();

            _dependencies.Contains<DefaultConsoleWriter>().Should().BeTrue();
        }
        
        [Fact]
        public void AddDefaultFileWriter()
        {
            _dependencies.AddDefaultFileLogWriter();

            _dependencies.Contains<DefaultFileWriter>().Should().BeTrue();
        }
        
        [Fact]
        public void AddDefaultEnrichers()
        {
            _dependencies.AddDefaultLogEnrichers();

            _dependencies.Contains<LogLevelEnricher>().Should().BeTrue();
            _dependencies.Contains<SenderEnricher>().Should().BeTrue();
            _dependencies.Contains<TimeStampEnricher>().Should().BeTrue();
        }
        
        [Fact]
        public void AddLogProvider()
        {
            _dependencies.AddLogging();
            _dependencies.Contains<ILogProvider>().Should().BeTrue();
        }

        [Fact]
        public void AddLoggerFactory()
        {
            _dependencies
                .AddLogging()
                .BuildProvider()
                .GetService<ILogger<LoggingInstallerShould>>().Should().NotBeNull();
        }

        [Fact]
        public void AddLogEnricher()
        {
            _dependencies.AddLogEnricher<TestEnricher>();

            _dependencies.Contains(typeof(ILogEnricher)).Should().BeTrue();
            _dependencies.Contains(typeof(TestEnricher)).Should().BeTrue();

            _dependencies.GetLifetime<ILogEnricher>().Should().Be(DependencyLifetime.Singleton);
            _dependencies.GetLifetime<TestEnricher>().Should().Be(DependencyLifetime.Singleton);
        }

        [Fact]
        public void AddLogEnricherInstance()
        {
            _dependencies.AddLogEnricher(new Mock<ILogEnricher>().Object);

            _dependencies.Contains(typeof(ILogEnricher)).Should().BeTrue();
            _dependencies.GetLifetime<ILogEnricher>().Should().Be(DependencyLifetime.Singleton);
        }

        [Theory, AutoData]
        public void AddLogEnricherWithLifetime(DependencyLifetime lifetime)
        {
            _dependencies.AddLogEnricher<TestEnricher>(lifetime);

            _dependencies.GetLifetime<TestEnricher>().Should().Be(lifetime);
        }
        
        [Fact]
        public void AddLogWriter()
        {
            _dependencies.AddLogWriter<TestWriter>();

            _dependencies.Contains(typeof(ILogWriter)).Should().BeTrue();
            _dependencies.Contains(typeof(TestWriter)).Should().BeTrue();

            _dependencies.GetLifetime<ILogWriter>().Should().Be(DependencyLifetime.Singleton);
            _dependencies.GetLifetime<TestWriter>().Should().Be(DependencyLifetime.Singleton);
        }

        [Fact]
        public void AddLogWriterInstance()
        {
            _dependencies.AddLogWriter(new Mock<ILogWriter>().Object);

            _dependencies.Contains(typeof(ILogWriter)).Should().BeTrue();
            _dependencies.GetLifetime<ILogWriter>().Should().Be(DependencyLifetime.Singleton);
        }

        [Theory]
        [InlineData(DependencyLifetime.Scoped)]
        [InlineData(DependencyLifetime.Singleton)]
        [InlineData(DependencyLifetime.Transient)]
        public void AddLogWriterWithLifetime(DependencyLifetime lifetime)
        {
            _dependencies.AddLogWriter<TestWriter>(lifetime);
            _dependencies.GetLifetime<TestWriter>().Should().Be(lifetime);
        }
        
        private sealed class TestEnricher : ILogEnricher
        {
            public void Enrich(LogLevel level, Type sender, JsonObject message)
            {
                throw new NotImplementedException();
            }
        }
        
        private sealed class TestWriter : ILogWriter
        {
            // ReSharper disable once UnassignedGetOnlyAutoProperty
            public LogLevel Level { get; }
            
            public void Write(LogContext context, JsonObject message)
            {
                throw new NotImplementedException();
            }
        }
    }
}