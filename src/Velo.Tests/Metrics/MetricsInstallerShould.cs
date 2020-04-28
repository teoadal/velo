using System;
using FluentAssertions;
using Velo.DependencyInjection;
using Velo.Metrics.Counters;
using Velo.Metrics.Provider;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Metrics
{
    public class MetricsInstallerShould : TestClass
    {
        private readonly DependencyCollection _dependencies;

        public MetricsInstallerShould(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection();
        }

        [Fact]
        public void AddService()
        {
            _dependencies.AddMetrics();
            _dependencies.Contains(typeof(IMetricsProvider)).Should().BeTrue();
        }

        [Fact]
        public void AddCounter()
        {
            _dependencies.AddMetricsCounter<MetricsInstallerShould>("abc", null, Array.Empty<ICounterLabel>());

            _dependencies.Contains(typeof(ICounter));
            _dependencies.Contains(typeof(ICounter<MetricsInstallerShould>));
        }
        
        [Fact]
        public void AddCounterInstance()
        {
            _dependencies.AddMetricsCounter(CreateCounter());

            _dependencies.Contains(typeof(ICounter));
            _dependencies.Contains(typeof(ICounter<MetricsInstallerShould>));
        }

        [Fact]
        public void AddCounterByFunction()
        {
            _dependencies.AddMetricsCounter(ctx => CreateCounter());

            _dependencies.Contains(typeof(ICounter));
            _dependencies.Contains(typeof(ICounter<MetricsInstallerShould>));
        }

        private ICounter<MetricsInstallerShould> CreateCounter()
        {
            return new Counter<MetricsInstallerShould>("abc", null, Array.Empty<ICounterLabel>());
        }
    }
}