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
        public void Instance()
        {
            var container = new DependencyBuilder()
                .Configure(c => c.Instance(new JConverter()))
                .BuildContainer();

            var instance1 = container.Resolve<JConverter>();
            var instance2 = container.Resolve<JConverter>();

            Assert.Same(instance1, instance2);
        }

        [Fact]
        public void Scope()
        {
            var container = new DependencyBuilder()
                .Configure(c => c
                    .Contract(typeof(IMapper<>))
                    .Implementation(typeof(CompiledMapper<>))
                    .Scope())
                .BuildContainer();

            IMapper<Boo> firstScopeMapper;
            using (container.StartScope())
            {
                firstScopeMapper = container.Resolve<IMapper<Boo>>();
            }

            using (container.StartScope())
            {
                Assert.NotSame(firstScopeMapper, container.Resolve<IMapper<Boo>>());
            }
        }

        [Fact]
        public void Scope_Builder()
        {
            var container = new DependencyBuilder()
                .Configure(c => c
                    .Contract<ISession>()
                    .Builder(ctx => new Session(new JConverter()))
                    .Scope())
                .BuildContainer();

            ISession firstScopeSession;
            using (container.StartScope())
            {
                firstScopeSession = container.Resolve<ISession>();
            }

            using (container.StartScope())
            {
                Assert.NotSame(firstScopeSession, container.Resolve<ISession>());
            }
        }

        [Fact]
        public void Singleton()
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
        public void Singleton_Builder()
        {
            var container = new DependencyBuilder()
                .Configure(c => c
                    .Builder(ctx => new JConverter())
                    .Singleton())
                .BuildContainer();

            var instance1 = container.Resolve<JConverter>();
            var instance2 = container.Resolve<JConverter>();

            Assert.Same(instance1, instance2);
        }
        
        [Fact]
        public void Singleton_Generic()
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
        public void Singleton_Other()
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
        public void Singleton_Two_Contracts()
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
        public void Throw_Exists_Builder_Or_Implementation()
        {
            Assert.Throws<InvalidOperationException>(() => new DependencyBuilder()
                .Configure(c => c
                    .Contracts<IRepository, IBooRepository>()
                    .Implementation<BooRepository>()
                    .Builder(ctx => new BooRepository(null, null))
                    .Scope()));
            
            Assert.Throws<InvalidOperationException>(() => new DependencyBuilder()
                .Configure(c => c
                    .Contracts<IRepository, IBooRepository>()
                    .Instance(new BooRepository(null, null))
                    .Implementation(typeof(BooRepository))
                    .Singleton()));
        }
        
        [Fact]
        public void Throw_Invalid_Contract()
        {
            Assert.Throws<InvalidOperationException>(() => new DependencyBuilder()
                .Configure(c => c
                    .Implementation<BooService>()
                    .Contracts<IRepository, IBooRepository>()));
        }
        
        [Fact]
        public void Throw_Invalid_Implementation()
        {
            Assert.Throws<InvalidOperationException>(() => new DependencyBuilder()
                .Configure(c => c
                    .Contracts<IRepository, IBooRepository>()
                    .Implementation<BooService>()));
        }
        
        [Fact]
        public void Throw_Invalid_Lifetimes()
        {
            Assert.Throws<InvalidOperationException>(() => new DependencyBuilder()
                .Configure(c => c
                    .Contracts<IRepository, IBooRepository>()
                    .Implementation<BooRepository>()
                    .Scope()
                    .Singleton()));

            Assert.Throws<InvalidOperationException>(() => new DependencyBuilder()
                .Configure(c => c
                    .Contracts<IRepository, IBooRepository>()
                    .Implementation<BooRepository>()
                    .Scope()
                    .Transient()));
        }
        
        [Fact]
        public void Throw_Name_Exists()
        {
            Assert.Throws<InvalidOperationException>(() => new DependencyBuilder()
                .Configure(c => c
                    .Name("name")
                    .Name("secondName")));
        }
        
        [Fact]
        public void Throw_No_Builder_Or_Implementation()
        {
            Assert.Throws<InvalidOperationException>(() => new DependencyBuilder()
                .Configure(c => c
                    .Contracts<IRepository, IBooRepository>()
                    .Scope()));
            
            Assert.Throws<InvalidOperationException>(() => new DependencyBuilder()
                .Configure(c => c
                    .Contracts<IRepository, IBooRepository>()
                    .Singleton()));
            
            Assert.Throws<InvalidOperationException>(() => new DependencyBuilder()
                .Configure(c => c
                    .Contracts<IRepository, IBooRepository>()
                    .Transient()));
        }

        [Fact]
        public void Transient()
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
        public void Transient_Builder()
        {
            var container = new DependencyBuilder()
                .Configure(c => c
                    .Contracts<ISession, Session>()
                    .Builder(ctx => new Session(new JConverter()))
                    .Transient())
                .BuildContainer();

            var instance1 = container.Resolve<Session>();
            var instance2 = container.Resolve<Session>();

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