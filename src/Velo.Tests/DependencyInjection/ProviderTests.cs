using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Velo.DependencyInjection;
using Velo.Mapping;
using Velo.Serialization;
using Velo.Settings.Provider;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Velo.Utils;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection
{
    public class ProviderTests : TestClass
    {
        private readonly DependencyCollection _dependencies;

        public ProviderTests(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection()
                .AddJsonConverter()
                .AddSingleton<ISettingsProvider>(ctx => new NullSettingsProvider())
                .AddSingleton<ISession, Session>();
        }

        [Fact]
        public void Activate()
        {
            var provider = _dependencies.BuildProvider();
            var repository = provider.Activate(typeof(FooRepository));
            
            Assert.NotNull(repository);
            Assert.IsType<FooRepository>(repository);
        }
        
        [Fact]
        public void ActivateGeneric()
        {
            var provider = _dependencies.BuildProvider();
            var repository = provider.Activate<FooRepository>();
            
            Assert.NotNull(repository);
            Assert.IsType<FooRepository>(repository);
        }
        
        [Fact]
        public void ActivateGenericWithConstructor()
        {
            var provider = _dependencies.BuildProvider();
            var repositoryConstructor = ReflectionUtils.GetConstructor(typeof(FooRepository));
            var repository = provider.Activate<FooRepository>(repositoryConstructor);
            
            Assert.NotNull(repository);
            Assert.IsType<FooRepository>(repository);
        }
        
        [Fact]
        public void Destroy()
        {
            var provider = _dependencies
                .AddSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .AddSingleton<IBooRepository>(ctx => new BooRepository(ctx.GetService<ISettingsProvider>(), ctx.GetService<ISession>()))
                .AddSingleton<BooService>()
                .BuildProvider();

            var repository = (BooRepository) provider.GetRequiredService<IBooRepository>();
            var service = provider.GetRequiredService<BooService>();

            provider.Dispose();

            Assert.True(repository.Disposed);
            Assert.True(service.Disposed);
        }

        [Fact]
        public void Resolve()
        {
            var provider = new DependencyCollection()
                .AddSingleton<JConverter>()
                .AddSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddSingleton<ISettingsProvider>(ctx => new NullSettingsProvider())
                .AddTransient<ISession, Session>()
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IFooRepository, FooRepository>()
                .AddSingleton<IBooService, BooService>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddSingleton<SomethingController>()
                .AddLogging()
                .BuildProvider();

            var controller = provider.GetService<SomethingController>();
            Assert.NotNull(controller);
        }

        [Fact]
        public void Resolve_DependencyProvider()
        {
            var provider = _dependencies.BuildProvider();

            Assert.Equal(provider, provider.GetRequiredService<DependencyProvider>());
            Assert.Equal(provider, provider.GetRequiredService<IServiceProvider>());
        }

        [Fact]
        public void Resolve_Array()
        {
            var provider = _dependencies
                .AddSingleton<IRepository, BooRepository>()
                .AddSingleton<IRepository, FooRepository>()
                .AddSingleton<IRepository, OtherFooRepository>()
                .BuildProvider();

            var array = provider.GetService<IRepository[]>();

            Assert.Equal(3, array.Length);
        }

        [Fact]
        public void Resolve_Array_Empty()
        {
            var provider = _dependencies.BuildProvider();
            var emptyArray = provider.GetRequiredService<IRepository[]>();
            Assert.Empty(emptyArray);
        }
        
        [Fact]
        public void Resolve_FirstFromAssignable()
        {
            var provider = _dependencies
                .AddSingleton<IRepository, BooRepository>()
                .AddSingleton<IRepository, FooRepository>()
                .BuildProvider();

            var firstAdded = provider.GetRequiredService<IRepository>();
            Assert.IsType<BooRepository>(firstAdded);
            
            var notFound = provider.GetService<BooRepository>();
            Assert.Null(notFound);
        }
        
        [Fact]
        public void Resolve_Enumerable()
        {
            var provider = _dependencies
                .AddSingleton<IRepository, BooRepository>()
                .AddSingleton<IRepository, FooRepository>()
                .AddSingleton<IRepository, OtherFooRepository>()
                .BuildProvider();

            var array = provider.GetService<IEnumerable<IRepository>>();
            Assert.Equal(3, array.Count());
        }
        
        [Fact]
        public async Task Resolve_Array_MultiThreading()
        {
            var provider = _dependencies
                .AddSingleton<IRepository, BooRepository>()
                .AddSingleton<IRepository, FooRepository>()
                .AddSingleton<IRepository, OtherFooRepository>()
                .BuildProvider();


            await RunTasks(10, () =>
            {
                var array = provider.GetService<IRepository[]>();
                Assert.Equal(3, array.Length);
            });
        }
        
        [Fact]
        public void Resolve_Array_WithOneElement()
        {
            var provider = _dependencies
                .AddSingleton<IRepository, BooRepository>()
                .BuildProvider();

            var array = provider.GetService<IRepository[]>();

            Assert.Single(array);
            Assert.IsType<BooRepository>(array[0]);
        }

        [Fact]
        public async Task Resolve_MultiThreading()
        {
            var provider = _dependencies
                .AddSingleton(typeof(IMapper<>), typeof(CompiledMapper<>))
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IFooRepository, FooRepository>()
                .AddSingleton<IBooService, BooService>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddSingleton<SomethingController>()
                .AddLogging()
                .BuildProvider();

            var resolvedControllers = new ConcurrentBag<SomethingController>();

            await RunTasks(10, () =>
            {
                var controller = provider.GetService<SomethingController>();
                resolvedControllers.Add(controller);
            });
            
            foreach (var controller in resolvedControllers)
            {
                foreach (var otherController in resolvedControllers)
                {
                    Assert.Same(controller, otherController);
                    Assert.Same(controller.Logger, otherController.Logger);
                    Assert.Same(controller.BooService, otherController.BooService);
                    Assert.Same(controller.FooService, otherController.FooService);
                    Assert.Same(controller.FooService.Settings, otherController.FooService.Settings);
                    Assert.Same(controller.FooService.Mapper, otherController.FooService.Mapper);
                    Assert.Same(controller.FooService.Repository, otherController.FooService.Repository);
                }
            }
        }

        [Fact]
        public void Throw_CircularDependency()
        {
            var provider = _dependencies
                .AddSingleton<CircularDependencyService>()
                .BuildProvider();
            
            Assert.Throws<TypeAccessException>(() => provider.GetService<CircularDependencyService>());
        }
        
        [Fact]
        public void Throw_CircularDependency_Required()
        {
            var provider = _dependencies
                .AddSingleton<CircularDependencyService>()
                .BuildProvider();
            
            Assert.Throws<TypeAccessException>(() => provider.GetRequiredService<CircularDependencyService>());
        }

        [Fact]
        public void Throw_Disposed()
        {
            var provider = new DependencyCollection()
                .AddSingleton<JConverter>()
                .BuildProvider();

            provider.Dispose();

            Assert.Throws<ObjectDisposedException>(() => provider.GetRequiredService<JConverter>());
        }

        [Fact]
        public void Throw_Activate_Interface()
        {
            var provider = new DependencyCollection().BuildProvider();
            Assert.Throws<InvalidOperationException>(() => provider.Activate<ISession>());
        }
        
        [Fact]
        public void Throw_Activate_NotRegistered()
        {
            var provider = new DependencyCollection().BuildProvider();
            Assert.Throws<KeyNotFoundException>(() => provider.Activate<FooRepository>());
        }
        
        [Fact]
        public void Throw_Resolve_NotRegistered()
        {
            var provider = _dependencies.BuildProvider();
            Assert.Throws<KeyNotFoundException>(() => provider.GetRequiredService(typeof(IManager<>)));
        }
    }
}