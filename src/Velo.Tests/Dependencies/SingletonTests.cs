using Velo.Mapping;
using Velo.Serialization;
using Velo.TestsModels.Boos;
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
        public void Singleton_Activator()
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
        public void Singleton_Builder()
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
        public void Singleton_Generic()
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
        public void Singleton_Generic_With_Contract()
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
        public void Singleton_Instance()
        {
            var container = new DependencyBuilder()
                .AddInstance(new JConverter())
                .BuildContainer();

            var first = container.Resolve<JConverter>();
            var second = container.Resolve<JConverter>();

            Assert.Same(first, second);
        }
    }
}