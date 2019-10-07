using System;
using Velo.Mapping;
using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Dependencies
{
    public class SingletonTests : TestBase
    {
        public SingletonTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Activator()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddSingleton<ISession, Session>()
                .BuildContainer();

            var first = container.Resolve<ISession>();
            var second = container.Resolve<ISession>();

            Assert.Same(first, second);
        }

        [Fact]
        public void Builder()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddSingleton<ISession>(ctx => new Session(ctx.Resolve<JConverter>()))
                .BuildContainer();

            var first = container.Resolve<ISession>();
            var second = container.Resolve<ISession>();

            Assert.Same(first, second);
        }

        [Fact]
        public void Builder_Destroy()
        {
            var container = new DependencyBuilder()
                .AddSingleton<IManager<Boo>>(ctx => new Manager<Boo>())
                .BuildContainer();

            var manager = container.Resolve<IManager<Boo>>();
            
            container.Destroy();

            Assert.True(manager.Disposed);
        }
        
        [Fact]
        public void Generic()
        {
            var container = new DependencyBuilder()
                .AddGenericSingleton(typeof(CompiledMapper<>))
                .BuildContainer();

            var boo1 = container.Resolve<CompiledMapper<Boo>>();
            var boo2 = container.Resolve<CompiledMapper<Boo>>();

            Assert.Same(boo1, boo2);

            var foo1 = container.Resolve<CompiledMapper<Foo>>();
            var foo2 = container.Resolve<CompiledMapper<Foo>>();

            Assert.Same(foo1, foo2);
        }

        [Fact]
        public void Generic_Destroy()
        {
            var container = new DependencyBuilder()
                .AddSingleton<ISession, Session>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddGenericSingleton(typeof(IManager<>), typeof(Manager<>))
                .BuildContainer();

            var manager1 = container.Resolve<IManager<Boo>>();
            var manager2 = container.Resolve<IManager<Foo>>();
            
            container.Destroy();
            
            Assert.True(manager1.Disposed);
            Assert.True(manager2.Disposed);
        }
        
        [Fact]
        public void Generic_With_Contract()
        {
            var container = new DependencyBuilder()
                .AddGenericSingleton(typeof(IMapper<>), typeof(CompiledMapper<>))
                .BuildContainer();

            var boo1 = container.Resolve<IMapper<Boo>>();
            var boo2 = container.Resolve<IMapper<Boo>>();

            Assert.Same(boo1, boo2);

            var foo1 = container.Resolve<IMapper<Foo>>();
            var foo2 = container.Resolve<IMapper<Foo>>();

            Assert.Same(foo1, foo2);
        }

        [Fact]
        public void Generic_Not_Generic_Contract()
        {
            var builder = new DependencyBuilder();

            Assert.Throws<InvalidOperationException>(() =>
                builder.AddGenericSingleton(typeof(IFooRepository), typeof(FooRepository)));
        }
        
        [Fact]
        public void Generic_Not_Generic_Implementation()
        {
            var builder = new DependencyBuilder();

            Assert.Throws<InvalidOperationException>(() =>
                builder.AddGenericSingleton(typeof(IRepository<>), typeof(FooRepository)));
        }
        
        [Fact]
        public void Instance()
        {
            var container = new DependencyBuilder()
                .AddInstance(new JConverter())
                .BuildContainer();

            var first = container.Resolve<JConverter>();
            var second = container.Resolve<JConverter>();

            Assert.Same(first, second);
        }
        
        [Fact]
        public void Instance_Destroy()
        {
            var container = new DependencyBuilder()
                .AddInstance(new BooRepository(null, null))
                .BuildContainer();

            var repository = container.Resolve<BooRepository>();
            
            container.Destroy();
            
            Assert.True(repository.Disposed);
        }
    }
}