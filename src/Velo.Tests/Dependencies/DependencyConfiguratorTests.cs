using System;
using Velo.Mapping;
using Velo.Serialization;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Dependencies
{
    public class DependencyConfiguratorTests : TestBase
    {
        public DependencyConfiguratorTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Configurator_Instance()
        {
            var container = new DependencyBuilder()
                .Configure(c => c.Instance(new JConverter()))
                .BuildContainer();

            var instance1 = container.Resolve<JConverter>();
            var instance2 = container.Resolve<JConverter>();

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void Configurator_Singleton_Generic()
        {
            var container = new DependencyBuilder()
                .Configure(c => c
                    .Contract(typeof(IMapper<>))
                    .Implementation(typeof(CompiledMapper<>))
                    .Singleton())
                .BuildContainer();

            var booMapper1 = container.Resolve<IMapper<Boo>>();
            var booMapper2 = container.Resolve<IMapper<Boo>>();

            Assert.Same(booMapper1, booMapper2);

            var fooMapper1 = container.Resolve<IMapper<Foo>>();
            var fooMapper2 = container.Resolve<IMapper<Foo>>();

            Assert.Same(fooMapper1, fooMapper2);
        }

        [Fact]
        public void Configurator_Singleton()
        {
            var container = new DependencyBuilder()
                .Configure(c => c
                    .Implementation<JConverter>()
                    .Singleton())
                .BuildContainer();

            var instance1 = container.Resolve<JConverter>();
            var instance2 = container.Resolve<JConverter>();

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void Configurator_Singleton_Other()
        {
            const string dependencyName = "testDependency";

            var container = new DependencyBuilder()
                .Configure(c => c
                    .Contract<IMapper<Boo>>()
                    .Name(dependencyName)
                    .Implementation<CompiledMapper<Boo>>()
                    .Singleton())
                .BuildContainer();

            var instance1 = container.Resolve<IMapper<Boo>>();
            var instance2 = container.Resolve<IMapper<Boo>>(dependencyName);

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void Configurator_Two_Contracts_Singleton()
        {
            var container = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .Configure(c => c
                    .Contracts<IBooRepository, IRepository>()
                    .Implementation<BooRepository>()
                    .Singleton())
                .BuildContainer();

            var instance1 = container.Resolve<IRepository>();
            var instance2 = container.Resolve<IBooRepository>();

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void Configurator_Transient()
        {
            var container = new DependencyBuilder()
                .Configure(c => c
                    .Implementation<JConverter>()
                    .Transient())
                .BuildContainer();

            var instance1 = container.Resolve<JConverter>();
            var instance2 = container.Resolve<JConverter>();

            Assert.NotSame(instance1, instance2);
        }
        
        [Fact]
        public void Dependency_Implementation()
        {
            var container = new DependencyBuilder()
                .AddDependency(new RandomDependency())
                .BuildContainer();

            var random = container.Resolve<Random>();
            Assert.NotNull(random);
        }
        
        [Fact]
        public void Dependency_Implementation_WithName()
        {
            const string name = "random";
            
            var container = new DependencyBuilder()
                .AddDependency(new RandomDependency(), name)
                .BuildContainer();

            var random = container.Resolve<Random>(name);
            Assert.NotNull(random);
        }
    }
}