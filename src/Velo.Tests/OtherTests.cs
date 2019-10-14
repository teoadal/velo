using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Velo.Mapping;
using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;

namespace Velo
{
    public class OtherTests
    {
        private readonly ServiceProvider _container;

        public OtherTests()
        {
            _container = new ServiceCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddScoped<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddScoped<IFooService, FooService>()
                .AddScoped<IFooRepository, FooRepository>()
                .BuildServiceProvider();
        }

        [Fact]
        public void AspNetDI()
        {
            const int taskCount = 10;

            var fooServices = new ConcurrentBag<IFooService>();
            using (_container.CreateScope())
            {
                var tasks = new Task[taskCount];
                for (var i = 0; i < taskCount; i++)
                {
                    tasks[i] = Task.Run(() => fooServices.Add(_container.GetService<IFooService>()));
                }

                Task.WaitAll(tasks);
            }
            
            var services = fooServices.ToArray();
            for (var i = 0; i < services.Length; i++)
            for (var j = 0; j < services.Length; j++)
            {
                if (i == j) continue;
                Assert.Same(services[i], services[j]);
            }
        }

        [Fact]
        public void AspNetDI_Array()
        {
            var container = new ServiceCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .AddScoped<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddScoped<IRepository, BooRepository>()
                .AddSingleton<IRepository, FooRepository>()
                .AddTransient<IRepository, OtherFooRepository>()
                .BuildServiceProvider();

            var first = container.GetService<IRepository>();
            Assert.NotNull(first);
            
            var array = container.GetService<IRepository[]>();
            Assert.Null(array);
            
            var enumerable = container.GetService<IEnumerable<IRepository>>();
            Assert.Equal(3, enumerable.ToArray().Length);
        }
        
        [Fact]
        public void AspNetDI_Scope_On_Scope()
        {
            using (var scope = _container.CreateScope())
            {
                var scopeProvider = scope.ServiceProvider;
                var session = scopeProvider.GetService<ISession>();
                
                Assert.Same(session, scopeProvider.GetService<ISession>());

                using (var nestedSCope = _container.CreateScope())
                {
                    var nestedScopeProvider = nestedSCope.ServiceProvider;
                    Assert.NotSame(session, nestedScopeProvider.GetService<ISession>());
                }
            }
        }
        
        [Fact]
        public void AspNetDI_Scope_On_Task()
        {
            const int scopesCount = 10;

            var session = _container.GetService<ISession>();
            var fooServices = new ConcurrentBag<IFooService>();
            var tasks = new Task[scopesCount];
            for (var i = 0; i < scopesCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    using (var scope = _container.CreateScope())
                    {
                        var scopeProvider = scope.ServiceProvider;
                        Assert.NotSame(session, scopeProvider.GetService<ISession>());
                        
                        fooServices.Add(scopeProvider.GetService<IFooService>());
                    }
                });
            }

            Task.WaitAll(tasks);

            var services = fooServices.ToArray();
            for (var i = 0; i < services.Length; i++)
            for (var j = 0; j < services.Length; j++)
            {
                if (i == j) continue;
                Assert.NotSame(services[i], services[j]);
            }
        }
        
    }
}