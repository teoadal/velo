using System;
using System.Collections.Generic;
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
    public class DependencyContainerTests : TestBase
    {
        private readonly DependencyBuilder _builder;

        public DependencyContainerTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>();
        }

        [Fact]
        public void As_ServiceProvider()
        {
            IServiceProvider container = _builder.BuildContainer();

            var session = container.GetService(typeof(ISession));

            Assert.NotNull(session);
            Assert.IsAssignableFrom<ISession>(session);
        }

        [Fact]
        public void Activate()
        {
            var container = _builder.BuildContainer();

            var repository1 = container.Activate<BooRepository>();
            var repository2 = container.Activate<BooRepository>();

            Assert.NotSame(repository1, repository2);
            Assert.Same(repository1.Configuration, repository2.Configuration);
            Assert.Same(repository1.Session, repository2.Session);
        }

        [Fact]
        public void Circular_Dependency_Detection()
        {
            var container = _builder
                .AddSingleton<CircularDependencyService>()
                .BuildContainer();

            Assert.Throws<TypeAccessException>(() => container.Resolve<CircularDependencyService>());
        }

        [Fact]
        public void CreateActivator()
        {
            var container = _builder.BuildContainer();

            var activator = container.CreateActivator<BooRepository>();

            var repository1 = activator();
            var repository2 = activator();

            Assert.NotSame(repository1, repository2);
            Assert.Same(repository1.Configuration, repository2.Configuration);
            Assert.Same(repository1.Session, repository2.Session);
        }

        [Fact]
        public void Destroy()
        {
            var container = _builder
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
        public void GetDependency()
        {
            var container = _builder.BuildContainer();

            var contract = typeof(ISession);
            var dependency = container.GetDependency(contract);
            var session = dependency.Resolve(contract, container);

            Assert.NotNull(dependency);
            Assert.NotNull(session);
            Assert.IsAssignableFrom<ISession>(session);
        }

        [Fact]
        public void GetDependency_By_Name()
        {
            const string booRepositoryName = "booRepository";
            const string fooRepositoryName = "fooRepository";

            var container = _builder
                .AddSingleton<IRepository, BooRepository>(booRepositoryName)
                .AddSingleton<IRepository, FooRepository>(fooRepositoryName)
                .BuildContainer();

            var contract = typeof(IRepository);

            var booDependency = container.GetDependency(contract, booRepositoryName);
            Assert.IsType<BooRepository>(booDependency.Resolve(contract, container));

            var fooDependency = container.GetDependency(contract, fooRepositoryName);
            Assert.IsType<FooRepository>(fooDependency.Resolve(contract, container));
        }

        [Fact]
        public void GetDependency_Not_Registered()
        {
            var container = _builder.BuildContainer();

            var dependency = container.GetDependency(typeof(IManager<>), throwInNotRegistered: false);

            Assert.Null(dependency);
        }

        [Fact]
        public void GetDependency_Not_Registered_ByName()
        {
            var container = _builder
                .AddTransient<IRepository, BooRepository>("name")
                .BuildContainer();

            var dependency = container.GetDependency(typeof(IRepository), "differentName");

            Assert.NotNull(dependency);
        }

        [Fact]
        public void GetDependency_Throw_Not_Registered()
        {
            var container = _builder.BuildContainer();

            Assert.Throws<KeyNotFoundException>(() => container.GetDependency(typeof(IManager<>)));
        }

        [Fact]
        public void Inject_By_Name()
        {
            const string booRepositoryName = "booRepository";
            const string fooRepositoryName = "fooRepository";

            var container = _builder
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

            var container = _builder
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

            var container = _builder
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

            var container = _builder
                .AddSingleton<IRepository, FooRepository>(fooRepositoryName)
                .BuildContainer();

            var repository = container.Resolve<IRepository>();
            Assert.IsType<FooRepository>(repository);
        }
    }
}