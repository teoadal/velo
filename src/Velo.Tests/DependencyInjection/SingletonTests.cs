using System;
using System.Threading.Tasks;
using Velo.Mapping;
using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.DependencyInjection
{
    public class SingletonTests : TestBase
    {
        private readonly DependencyCollection _collection;

        public SingletonTests(ITestOutputHelper output) : base(output)
        {
            _collection = new DependencyCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<JConverter>();
        }

        [Fact]
        public void Activator()
        {
            var provider = _collection
                .AddSingleton<ISession, Session>()
                .BuildProvider();

            var first = provider.GetService<ISession>();
            var second = provider.GetService<ISession>();

            Assert.Same(first, second);
        }

        [Fact]
        public void Activator_Destroy()
        {
            var provider = _collection
                .AddSingleton<Manager<Boo>>()
                .BuildProvider();

            var manager = provider.GetService<Manager<Boo>>();

            provider.Dispose();

            Assert.True(manager.Disposed);
        }

        [Fact]
        public async Task Activator_MultiThreading()
        {
            var provider = _collection
                .AddSingleton<ISession, Session>()
                .AddSingleton<IFooRepository, FooRepository>()
                .AddGenericSingleton(typeof(IMapper<>), typeof(CompiledMapper<>))
                .AddSingleton<FooService>()
                .BuildProvider();


            var firstTask = Task.Run(() => provider.GetService<FooService>());
            var secondTask = Task.Run(() => provider.GetService<FooService>());

            await Task.WhenAll(firstTask, secondTask);

            var firstService = firstTask.Result;
            var secondService = secondTask.Result;
            Assert.Same(firstService, secondService);
            Assert.Same(firstService.Configuration, secondService.Configuration);
            Assert.Same(firstService.Mapper, secondService.Mapper);
            Assert.Same(firstService.Repository, secondService.Repository);
            Assert.Same(firstService.Repository.Session, secondService.Repository.Session);
        }

        [Fact]
        public void Builder()
        {
            var provider = _collection
                .AddSingleton<ISession>(ctx => new Session(ctx.GetService<JConverter>()))
                .BuildProvider();

            var first = provider.GetService<ISession>();
            var second = provider.GetService<ISession>();

            Assert.Same(first, second);
        }

        [Fact]
        public void Builder_Destroy()
        {
            var provider = _collection
                .AddSingleton<IManager<Boo>>(ctx => new Manager<Boo>())
                .BuildProvider();

            var manager = provider.GetService<IManager<Boo>>();

            provider.Dispose();

            Assert.True(manager.Disposed);
        }

        [Fact]
        public async Task Builder_MultiThreading()
        {
            var provider = _collection
                .AddSingleton<ISession, Session>()
                .AddSingleton<IFooRepository, FooRepository>()
                .AddGenericSingleton(typeof(IMapper<>), typeof(CompiledMapper<>))
                .AddSingleton(ctx => new FooService(
                    ctx.GetService<IConfiguration>(),
                    ctx.GetService<IMapper<Foo>>(),
                    ctx.GetService<IFooRepository>()))
                .BuildProvider();
            
            var firstTask = Task.Run(() => provider.GetService<FooService>());
            var secondTask = Task.Run(() => provider.GetService<FooService>());

            await Task.WhenAll(firstTask, secondTask);

            var firstService = firstTask.Result;
            var secondService = secondTask.Result;
            Assert.Same(firstService, secondService);
            Assert.Same(firstService.Configuration, secondService.Configuration);
            Assert.Same(firstService.Mapper, secondService.Mapper);
            Assert.Same(firstService.Repository, secondService.Repository);
            Assert.Same(firstService.Repository.Session, secondService.Repository.Session);
        }

        [Fact]
        public void Instance()
        {
            var provider = _collection
                .AddInstance(new JConverter())
                .BuildProvider();

            var first = provider.GetService<JConverter>();
            var second = provider.GetService<JConverter>();

            Assert.Same(first, second);
        }

        [Fact]
        public void Instance_Destroy()
        {
            var provider = _collection
                .AddInstance(new BooRepository(null, null))
                .BuildProvider();

            var repository = provider.GetService<BooRepository>();

            provider.Dispose();

            Assert.True(repository.Disposed);
        }

        [Fact]
        public void Generic()
        {
            var provider = _collection
                .AddGenericSingleton(typeof(CompiledMapper<>))
                .BuildProvider();

            var boo1 = provider.GetService<CompiledMapper<Boo>>();
            var boo2 = provider.GetService<CompiledMapper<Boo>>();

            Assert.Same(boo1, boo2);

            var foo1 = provider.GetService<CompiledMapper<Foo>>();
            var foo2 = provider.GetService<CompiledMapper<Foo>>();

            Assert.Same(foo1, foo2);
        }

        [Fact]
        public void Generic_Destroy()
        {
            var provider = _collection
                .AddSingleton<ISession, Session>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddGenericSingleton(typeof(IManager<>), typeof(Manager<>))
                .BuildProvider();

            var manager1 = provider.GetService<IManager<Boo>>();
            var manager2 = provider.GetService<IManager<Foo>>();

            provider.Dispose();

            Assert.True(manager1.Disposed);
            Assert.True(manager2.Disposed);
        }

        [Fact]
        public void Generic_WithContract()
        {
            var provider = _collection
                .AddGenericSingleton(typeof(IMapper<>), typeof(CompiledMapper<>))
                .BuildProvider();

            var boo1 = provider.GetService<IMapper<Boo>>();
            var boo2 = provider.GetService<IMapper<Boo>>();

            Assert.Same(boo1, boo2);

            var foo1 = provider.GetService<IMapper<Foo>>();
            var foo2 = provider.GetService<IMapper<Foo>>();

            Assert.Same(foo1, foo2);
        }

        [Fact]
        public void Throw_Not_Generic_Contract()
        {
            var builder = new DependencyCollection();

            Assert.Throws<InvalidOperationException>(() =>
                builder.AddGenericSingleton(typeof(IFooRepository), typeof(FooRepository)));
        }

        [Fact]
        public void Throw_Not_Generic_Implementation()
        {
            var builder = new DependencyCollection();

            Assert.Throws<InvalidOperationException>(() =>
                builder.AddGenericSingleton(typeof(IRepository<>), typeof(FooRepository)));
        }
    }
}