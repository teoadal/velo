using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Velo.CQRS;
using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.TestsModels.Boos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Extensions.DependencyInjection
{
    public class ServiceCollectionExtensionsShould : TestClass
    {
        public ServiceCollectionExtensionsShould(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void AddServicesToDependencies()
        {
            var provider = new DependencyCollection()
                .AddServiceCollection(new ServiceCollection()
                    .AddJsonConverter()
                    .AddEmitter())
                .BuildProvider();

            provider.GetRequiredService<JConverter>().Should().NotBeNull();
            provider.GetRequiredService<IEmitter>().Should().BeOfType<Emitter>();
        }
        
        [Fact]
        public void AddDependencyProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddDependencyProvider(dependencies => dependencies
                    .AddEmitter()
                    .AddLogging())
                .BuildServiceProvider();

            var dependencyProvider = serviceProvider.GetRequiredService<DependencyProvider>();
            dependencyProvider.GetRequiredService<IEmitter>();
        }
        
        [Fact]
        public void CreateDependencyProvider()
        {
            var provider = new ServiceCollection()
                .BuildDependencyProvider();

            provider.Should().BeOfType<DependencyProvider>();
        }

        [Fact]
        public void ContainsDependencies()
        {
            var provider = new ServiceCollection()
                .AddJsonConverter()
                .BuildDependencyProvider();

            provider.GetRequiredService<JConverter>();
            provider.GetRequiredService<IConvertersCollection>();
        }

        [Fact]
        public void RemoveDescriptor()
        {
            var services = new ServiceCollection()
                .AddSingleton<JConverter>();

            services
                .RemoveLessLifetimeService(typeof(JConverter), ServiceLifetime.Scoped)
                .Should().BeTrue();
        }
        
        [Fact]
        public void NotRemoveDescriptor()
        {
            var services = new ServiceCollection()
                .AddSingleton<JConverter>()
                .AddTransient<IBooRepository, BooRepository>();

            services
                .RemoveLessLifetimeService(typeof(JConverter), ServiceLifetime.Singleton)
                .Should().BeFalse();
            
            services
                .RemoveLessLifetimeService(typeof(IBooRepository), ServiceLifetime.Singleton)
                .Should().BeFalse();
        }
    }
}