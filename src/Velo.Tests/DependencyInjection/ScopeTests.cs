using System;
using System.Collections.Generic;
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
    public class ScopeTests : TestBase
    {
        private readonly DependencyCollection _builder;

        public ScopeTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<JConverter>();
        }

        [Fact]
        public void Activator()
        {
            var provider = _builder
                .AddScoped<ISession, Session>()
                .BuildProvider();

            var first = provider.GetService<ISession>();
            var second = provider.GetService<ISession>();

            Assert.Same(first, second);

            using (var scope = provider.CreateScope())
            {
                Assert.NotSame(first, scope.GetService<ISession>());
            }
        }

        [Fact]
        public void Activator_Destroy()
        {
            var provider = _builder
                .AddScoped<Manager<Boo>>()
                .BuildProvider();

            Manager<Boo> manager;
            using (var scope = provider.CreateScope())
            {
                manager = scope.GetService<Manager<Boo>>();
            }

            Assert.True(manager.Disposed);
        }
        
        [Fact]
        public void Activator_MultiThreading()
        {
            var provider = _builder
                .AddScoped<ISession, Session>()
                .AddScoped<IFooRepository, FooRepository>()
                .AddGenericScoped(typeof(IMapper<>), typeof(CompiledMapper<>))
                .AddScoped<FooService>()
                .BuildProvider();


            var tasks = new Task[10];
            for (var i = 0; i < 10; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    using (var scope = provider.CreateScope())
                    {
                        var firstService = scope.GetService<FooService>();
                        var secondService = scope.GetService<FooService>();            
                        Assert.Same(firstService, secondService);
                        Assert.Same(firstService.Mapper, secondService.Mapper);
                        Assert.Same(firstService.Repository, secondService.Repository);
                        Assert.Same(firstService.Repository.Session, secondService.Repository.Session);
                    }
                });
            }

            Task.WaitAll(tasks);
        }
        
        [Fact]
        public void Builder()
        {
            var provider = _builder
                .AddScoped<ISession>(ctx => new Session(ctx.GetService<JConverter>()))
                .BuildProvider();

            var first = provider.GetService<ISession>();
            var second = provider.GetService<ISession>();

            Assert.Same(first, second);
            
            using (var scope = provider.CreateScope())
            {
                Assert.NotSame(first, scope.GetService<ISession>());
            }
        }

        [Fact]
        public void Builder_Destroy()
        {
            var provider = _builder
                .AddScoped<IManager<Boo>>(ctx => new Manager<Boo>())
                .BuildProvider();

            IManager<Boo> manager;
            using (var scope = provider.CreateScope())
            {
                manager = scope.GetService<IManager<Boo>>();
            }

            Assert.True(manager.Disposed);
        }
        
        [Fact]
        public void Builder_MultiThreading()
        {
            var provider = _builder
                .AddScoped<ISession, Session>()
                .AddScoped<IFooRepository, FooRepository>()
                .AddGenericScoped(typeof(IMapper<>), typeof(CompiledMapper<>))
                .AddScoped(ctx => new FooService(
                    ctx.GetService<IConfiguration>(),
                    ctx.GetService<IMapper<Foo>>(),
                    ctx.GetService<IFooRepository>()))
                .BuildProvider();


            var tasks = new Task[10];
            for (var i = 0; i < 10; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    using (var scope = provider.CreateScope())
                    {
                        var firstService = scope.GetService<FooService>();
                        var secondService = scope.GetService<FooService>();            
                        Assert.Same(firstService, secondService);
                        Assert.Same(firstService.Mapper, secondService.Mapper);
                        Assert.Same(firstService.Repository, secondService.Repository);
                        Assert.Same(firstService.Repository.Session, secondService.Repository.Session);
                    }
                });
            }

            Task.WaitAll(tasks);
        }
        
        [Fact]
        public void Generic()
        {
            var provider = _builder
                .AddGenericScoped(typeof(List<>))
                .BuildProvider();

            var first = provider.GetService<List<int>>();
            var second = provider.GetService<List<int>>();

            Assert.Same(first, second);
            
            using (var scope = provider.CreateScope())
            {
                Assert.NotSame(first, scope.GetService<List<int>>());
            }
        }

        [Fact]
        public void Generic_Destroy()
        {
            var provider = _builder
                .AddGenericScoped(typeof(IManager<>), typeof(Manager<>))
                .BuildProvider();

            IManager<Boo> manager;
            using (var scope = provider.CreateScope())
            {
                manager = scope.GetService<IManager<Boo>>();
            }

            Assert.True(manager.Disposed);
        }
        
        [Fact]
        public void Generic_With_Contract()
        {
            var provider = _builder
                .AddGenericScoped(typeof(IList<>), typeof(List<>))
                .BuildProvider();

            var first = provider.GetService<IList<int>>();
            var second = provider.GetService<IList<int>>();

            Assert.Same(first, second);
            
            using (var scope = provider.CreateScope())
            {
                Assert.NotSame(first, scope.GetService<IList<int>>());
            }
        }

        [Fact]
        public void Throw_Not_Generic_Contract()
        {
            var builder = new DependencyCollection();

            Assert.Throws<InvalidOperationException>(() =>
                builder.AddGenericScoped(typeof(IFooRepository), typeof(FooRepository)));
        }

        [Fact]
        public void Throw_Not_Generic_Implementation()
        {
            var builder = new DependencyCollection();

            Assert.Throws<InvalidOperationException>(() =>
                builder.AddGenericScoped(typeof(IRepository<>), typeof(FooRepository)));
        }
    }
}