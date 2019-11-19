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
        private readonly DependencyCollection _dependencies;

        public ScopeTests(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<JConverter>();
        }

        [Fact]
        public void Compile()
        {
            var provider = _dependencies
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
        public void Compile_Destroy()
        {
            var provider = _dependencies
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
        public void Compile_MultiThreading()
        {
            var provider = _dependencies
                .AddScoped<ISession, Session>()
                .AddScoped<IFooRepository, FooRepository>()
                .AddScoped(typeof(IMapper<>), typeof(CompiledMapper<>))
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
            var provider = _dependencies
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
            var provider = _dependencies
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
        public async Task Builder_MultiThreading()
        {
            var provider = _dependencies
                .AddScoped<ISession, Session>()
                .AddScoped<IFooRepository, FooRepository>()
                .AddScoped(typeof(IMapper<>), typeof(CompiledMapper<>))
                .AddScoped(ctx => new FooService(
                    ctx.GetService<IConfiguration>(),
                    ctx.GetService<IMapper<Foo>>(),
                    ctx.GetService<IFooRepository>()))
                .BuildProvider();

            await RunTasks(10, () =>
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
        
        [Fact]
        public void Generic()
        {
            var provider = _dependencies
                .AddScoped(typeof(List<>))
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
            var provider = _dependencies
                .AddScoped(typeof(IManager<>), typeof(Manager<>))
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
            var provider = _dependencies
                .AddScoped(typeof(IList<>), typeof(List<>))
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
        public void Two_Contracts()
        {
            var implementation = typeof(FooRepository);
            var contracts = new[] {implementation, typeof(IRepository<Foo>)};

            var provider = _dependencies
                .AddSingleton<ISession, Session>()
                .AddDependency(contracts, implementation, DependencyLifetime.Scope)
                .BuildProvider();

            var byImplementation = provider.GetRequiredService<FooRepository>();
            var byInterface = provider.GetRequiredService<IRepository<Foo>>();
            
            Assert.Same(byImplementation, byInterface);
            Assert.IsType<FooRepository>(byInterface);
        }

        [Fact]
        public void Throw_Not_Generic_Implementation()
        {
            var builder = new DependencyCollection();

            Assert.Throws<InvalidOperationException>(() =>
                builder.AddScoped(typeof(IRepository<>), typeof(FooRepository)));
        }
    }
}