using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Velo.DependencyInjection;
using Velo.Logging;
using Velo.Logging.Enrichers;
using Velo.Logging.Provider;
using Velo.Logging.Writers;
using Velo.Serialization;
using Velo.Serialization.Models;
using Xunit;

namespace Velo.Tests.Logging
{
    public sealed class LoggingInstallerShould : TestClass
    {
        private readonly DependencyCollection _dependencies;

        public LoggingInstallerShould()
        {
            _dependencies = new DependencyCollection()
                .AddLogWriter(Mock.Of<ILogWriter>());
        }

        [Fact]
        public void AddDefaultConsoleWriter()
        {
            _dependencies.AddDefaultConsoleLogWriter();
            _dependencies.Contains<ILogWriter>().Should().BeTrue();
        }
        
        [Fact]
        public void AddDefaultFileWriter()
        {
            _dependencies.AddDefaultFileLogWriter();
            _dependencies.Contains<ILogWriter>().Should().BeTrue();
        }
        
        [Fact]
        public void AddDefaultEnrichers()
        {
            _dependencies.AddDefaultLogEnrichers();

            var dependencies = _dependencies.GetApplicable<ILogEnricher>();
            var implementations = dependencies.Select(d => d.Implementation).ToArray();

            implementations.Should().Contain(new []
            {
                typeof(LogLevelEnricher), 
                typeof(SenderEnricher),
                typeof(TimeStampEnricher)
            });
        }
        
        [Fact]
        public void AddLogProvider()
        {
            _dependencies.AddLogs();
            _dependencies.Contains<ILogProvider>().Should().BeTrue();
        }

        [Fact]
        public void AddLoggerFactory()
        {
            _dependencies
                .AddLogs()
                .BuildProvider()
                .Get<ILogger<LoggingInstallerShould>>().Should().NotBeNull();
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
            _dependencies.AddLogEnricher(Mock.Of<ILogEnricher>());

            _dependencies.Contains(typeof(ILogEnricher)).Should().BeTrue();
            _dependencies.GetLifetime<ILogEnricher>().Should().Be(DependencyLifetime.Singleton);
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
            _dependencies.AddLogWriter(Mock.Of<ILogWriter>());

            _dependencies.Contains(typeof(ILogWriter)).Should().BeTrue();
            _dependencies.GetLifetime<ILogWriter>().Should().Be(DependencyLifetime.Singleton);
        }

        [Fact]
        public void InstallJson()
        {
            _dependencies
                .AddLogs()
                .Contains<JConverter>().Should().BeTrue();
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