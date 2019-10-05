using System;
using System.Collections.Concurrent;
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

namespace Velo.Dependencies
{
    public class DependencyTests : TestBase
    {
        public DependencyTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Activate()
        {
            var container = new DependencyBuilder()
                .AddInstance(new JConverter())
                .AddTransient<ISession, Session>()
                .AddSingleton<IConfiguration, Configuration>()
                .BuildContainer();

            var repository1 = container.Activate<BooRepository>();
            var repository2 = container.Activate<BooRepository>();

            Assert.NotSame(repository1, repository2);
            Assert.Same(repository1.Configuration, repository2.Configuration);
            Assert.NotSame(repository1.Session, repository2.Session);
        }

        [Fact]
        public void Array_Factory()
        {
            var container = new DependencyBuilder()
                .AddTransient<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IConfiguration, Configuration>()
                .Configure(dataRepository => dataRepository
                    .Contracts<IRepository, IFooRepository>()
                    .Implementation<FooRepository>()
                    .Singleton())
                .Configure(userRepository => userRepository
                    .Contracts<IRepository, IBooRepository>()
                    .Implementation<BooRepository>()
                    .Singleton())
                .BuildContainer();

            var first = container.Resolve<IRepository[]>();
            var second = container.Resolve<IRepository[]>();

            Assert.NotSame(first, second);

            for (var i = 0; i < first.Length; i++)
            {
                var firstRepository = first[i];
                var secondRepository = second[i];

                Assert.Same(firstRepository, secondRepository);
                Assert.Same(firstRepository.Configuration, secondRepository.Configuration);
                Assert.Same(firstRepository.Session, secondRepository.Session);
            }
        }

        [Fact]
        public void Circular_Dependency()
        {
            var container = new DependencyBuilder()
                .AddSingleton<CircularDependencyService>()
                .BuildContainer();

            Assert.Throws<TypeAccessException>(() => container.Resolve<CircularDependencyService>());
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
        public void CreateActivator()
        {
            var container = new DependencyBuilder()
                .AddInstance(new JConverter())
                .AddTransient<ISession, Session>()
                .AddSingleton<IConfiguration, Configuration>()
                .BuildContainer();

            var activator = container.CreateActivator<BooRepository>();

            var repository1 = activator();
            var repository2 = activator();

            Assert.NotSame(repository1, repository2);
            Assert.Same(repository1.Configuration, repository2.Configuration);
            Assert.NotSame(repository1.Session, repository2.Session);
        }

        [Fact]
        public void Destroy()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddTransient<ISession, Session>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddSingleton<IFooRepository, FooRepository>()
                .AddSingleton<IFooService, FooService>()
                .BuildContainer();

            var service = container.Resolve<IFooService>();
            Assert.False(service.Disposed);

            container.Destroy();

            Assert.True(service.Disposed);
        }

        [Fact]
        public void Inject_By_Name()
        {
            const string booRepositoryName = "booRepository";
            const string fooRepositoryName = "fooRepository";

            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<IRepository, BooRepository>(booRepositoryName)
                .AddSingleton<IRepository, FooRepository>(fooRepositoryName)
                .BuildContainer();

            var repositoryCollection = container.Activate<RepositoryCollection>();

            Assert.IsType<BooRepository>(repositoryCollection.BooRepository);
            Assert.IsType<FooRepository>(repositoryCollection.FooRepository);
        }

        [Fact]
        public void Resolve()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddSingleton<IConfiguration>(provider => new Configuration())
                .AddTransient<ISession, Session>()
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IFooRepository, FooRepository>()
                .AddSingleton<IBooService, BooService>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddSingleton<SomethingController>()
                .BuildContainer();

            var controller = container.Resolve<SomethingController>();
            Assert.NotNull(controller);
        }

        [Fact]
        public void Resolve_By_Name()
        {
            const string booRepositoryName = "booRepository";
            const string fooRepositoryName = "fooRepository";

            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<IRepository, BooRepository>(booRepositoryName)
                .AddSingleton<IRepository, FooRepository>(fooRepositoryName)
                .BuildContainer();

            var booRepository = container.Resolve<IRepository>(booRepositoryName);
            var fooRepository = container.Resolve<IRepository>(fooRepositoryName);

            Assert.IsType<BooRepository>(booRepository);
            Assert.IsType<FooRepository>(fooRepository);
        }

        [Fact]
        public void Resolve_Generic_By_Name()
        {
            const string fooRepositoryName = "fooRepository";
            const string otherFooRepositoryName = "otherFooRepository";

            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<IRepository<Foo>, FooRepository>(fooRepositoryName)
                .AddSingleton<IRepository<Foo>, OtherFooRepository>(otherFooRepositoryName)
                .BuildContainer();

            var fooRepository = container.Resolve<IRepository<Foo>>(fooRepositoryName);
            var otherFooRepository = container.Resolve<IRepository<Foo>>(otherFooRepositoryName);

            Assert.IsType<FooRepository>(fooRepository);
            Assert.IsType<OtherFooRepository>(otherFooRepository);
        }

        [Fact]
        public void Resolve_Named_Without_Name()
        {
            const string fooRepositoryName = "fooRepository";
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<IRepository, FooRepository>(fooRepositoryName)
                .BuildContainer();

            var repository = container.Resolve<IRepository>();
            Assert.IsType<FooRepository>(repository);
        }

        [Fact]
        public void Scan_Assignable()
        {
            var container = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddTransient<ISession, Session>()
                .AddSingleton<JConverter>()
                .Scan(scanner => scanner
                    .Assembly(typeof(IRepository).Assembly)
                    .RegisterAsSingleton<IRepository>())
                .BuildContainer();

            var repositories = container.Resolve<IRepository[]>();
            Assert.Equal(3, repositories.Length);
            Assert.Contains(repositories, r => r.GetType() == typeof(BooRepository));
            Assert.Contains(repositories, r => r.GetType() == typeof(FooRepository));
            Assert.Contains(repositories, r => r.GetType() == typeof(OtherFooRepository));
        }

        [Fact]
        public void Scan_Generic_Interface_Implementations()
        {
            var container = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddTransient<ISession, Session>()
                .AddSingleton<JConverter>()
                .Scan(scanner => scanner
                    .Assembly(typeof(IRepository).Assembly)
                    .RegisterGenericInterfaceAsSingleton(typeof(IRepository<>)))
                .BuildContainer();

            var booRepository = container.Resolve<IRepository<Boo>>();
            Assert.NotNull(booRepository);

            var fooRepository = container.Resolve<IRepository<Foo>>();
            Assert.NotNull(fooRepository);
        }

        [Fact]
        public void Scope()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddScope<ISession, Session>()
                .BuildContainer();

            ISession firstScopeSession;
            using (container.StartScope())
            {
                firstScopeSession = container.Resolve<ISession>();
                Assert.Same(firstScopeSession, container.Resolve<ISession>());
            }

            using (container.StartScope())
            {
                var secondScopeSession = container.Resolve<ISession>();
                Assert.NotSame(firstScopeSession, secondScopeSession);
            }
        }

        [Fact]
        public void Scope_Circular_Dependency()
        {
            var container = new DependencyBuilder()
                .AddScope<CircularDependencyService>()
                .BuildContainer();

            using (container.StartScope())
            {
                Assert.Throws<TypeAccessException>(() => container.Resolve<CircularDependencyService>());
            }
        }

        [Fact]
        public void Scope_Destroy_After_End()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddTransient<ISession, Session>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddSingleton<IFooRepository, FooRepository>()
                .AddScope<IFooService, FooService>()
                .BuildContainer();

            IFooService service;
            using (container.StartScope())
            {
                service = container.Resolve<IFooService>();
                Assert.False(service.Disposed);
            }

            Assert.True(service.Disposed);
        }

        [Fact]
        public void Scope_Generic()
        {
            var container = new DependencyBuilder()
                .AddGenericScope(typeof(List<>))
                .BuildContainer();

            List<int> firstScopeList;
            using (container.StartScope())
            {
                firstScopeList = container.Resolve<List<int>>();
                Assert.Same(firstScopeList, container.Resolve<List<int>>());
            }

            using (container.StartScope())
            {
                var secondScopeList = container.Resolve<List<int>>();
                Assert.NotSame(firstScopeList, secondScopeList);
            }
        }

        [Fact]
        public void Scope_Generic_With_Contract()
        {
            var container = new DependencyBuilder()
                .AddGenericScope(typeof(IList<>), typeof(List<>))
                .BuildContainer();

            IList<int> firstScopeList;
            using (container.StartScope())
            {
                firstScopeList = container.Resolve<IList<int>>();
                Assert.Same(firstScopeList, container.Resolve<IList<int>>());
            }

            using (container.StartScope())
            {
                var secondScopeList = container.Resolve<IList<int>>();
                Assert.NotSame(firstScopeList, secondScopeList);
            }
        }

        [Fact]
        public void Scope_Many()
        {
            var container = new DependencyBuilder()
                .AddScope<IFooRepository, FooRepository>()
                .AddScope<IFooService, FooService>()
                .AddScope<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IConfiguration, Configuration>()
                .Configure(c => c
                    .Contract(typeof(IMapper<>))
                    .Implementation(typeof(CompiledMapper<>))
                    .Singleton())
                .BuildContainer();

            const int scopesCount = 10;

            var fooServices = new IFooService[scopesCount];
            for (var i = 0; i < fooServices.Length; i++)
            {
                using (container.StartScope())
                {
                    fooServices[i] = container.Resolve<IFooService>();
                }
            }

            for (var i = 0; i < fooServices.Length; i++)
            for (var j = 0; j < fooServices.Length; j++)
            {
                if (i == j) continue;
                Assert.NotSame(fooServices[i], fooServices[j]);
            }
        }

        [Fact]
        public void Scope_MultiThreading()
        {
            var container = new DependencyBuilder()
                .AddScope<IFooService, FooService>()
                .AddSingleton<IFooRepository, FooRepository>()
                .AddSingleton<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IConfiguration, Configuration>()
                .Configure(c => c
                    .Contract(typeof(IMapper<>))
                    .Implementation(typeof(CompiledMapper<>))
                    .Singleton())
                .BuildContainer();

            const int scopesCount = 10;

            var fooServices = new ConcurrentBag<IFooService>();
            var tasks = new Task[scopesCount];
            for (var i = 0; i < scopesCount; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    using (container.StartScope())
                    {
                        fooServices.Add(container.Resolve<IFooService>());
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

        [Fact]
        public void Scope_Nested()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddScope<ISession, Session>()
                .BuildContainer();

            using (container.StartScope())
            {
                var session = container.Resolve<ISession>();
                Assert.Same(session, container.Resolve<ISession>());

                using (container.StartScope())
                {
                    Assert.Same(session, container.Resolve<ISession>());
                }

                Assert.Same(session, container.Resolve<ISession>());
            }
        }

        [Fact]
        public void Scope_Throw_If_Resolve_WithoutScope()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddScope<ISession, Session>()
                .BuildContainer();

            Assert.Throws<InvalidOperationException>(() => container.Resolve<ISession>());
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

        [Fact]
        public void Transient()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddTransient<ISession, Session>()
                .BuildContainer();

            var first = container.Resolve<ISession>();
            var second = container.Resolve<ISession>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Transient_Builder()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddTransient<ISession>(ctx => new Session(ctx.Resolve<JConverter>()))
                .BuildContainer();

            var first = container.Resolve<ISession>();
            var second = container.Resolve<ISession>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Transient_Generic()
        {
            var container = new DependencyBuilder()
                .AddGenericTransient(typeof(List<>))
                .BuildContainer();

            var first = container.Resolve<List<int>>();
            var second = container.Resolve<List<int>>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Transient_Generic_With_Contract()
        {
            var container = new DependencyBuilder()
                .AddGenericTransient(typeof(IList<>), typeof(List<>))
                .BuildContainer();

            var first = container.Resolve<IList<int>>();
            var second = container.Resolve<IList<int>>();

            Assert.NotSame(first, second);
        }
    }
}