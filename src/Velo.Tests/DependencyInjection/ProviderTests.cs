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

namespace Velo.DependencyInjection
{
    public class ProviderTests : TestBase
    {
        private readonly DependencyCollection _builder;

        public ProviderTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyCollection()
                .AddSingleton<JConverter>()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<ISession, Session>();
        }

        [Fact]
        public void CircularDependency()
        {
            Assert.Throws<TypeAccessException>(() => _builder
                .AddSingleton<CircularDependencyService>()
                .BuildProvider());
        }

        [Fact]
        public void Destroy()
        {
            var provider = _builder
                .AddSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .AddSingleton<IBooRepository>(ctx =>
                    new BooRepository(ctx.GetService<IConfiguration>(), ctx.GetService<ISession>()))
                .AddSingleton<BooService>()
                .BuildProvider();

            var repository = (BooRepository) provider.GetService<IBooRepository>();
            var service = provider.GetService<BooService>();

            provider.Dispose();

            Assert.True(repository.Disposed);
            Assert.True(service.Disposed);
        }

        [Fact]
        public void Resolve()
        {
            var provider = new DependencyCollection()
                .AddSingleton<JConverter>()
                .AddSingleton<ILogger, Logger>()
                .AddSingleton<IMapper<Boo>, CompiledMapper<Boo>>()
                .AddSingleton<IMapper<Foo>, CompiledMapper<Foo>>()
                .AddSingleton<IConfiguration>(ctx => new Configuration())
                .AddTransient<ISession, Session>()
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IFooRepository, FooRepository>()
                .AddSingleton<IBooService, BooService>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddSingleton<SomethingController>()
                .BuildProvider();

            var controller = provider.GetService<SomethingController>();
            Assert.NotNull(controller);
        }

        [Fact]
        public void Resolve_Array()
        {
            var provider = _builder
                .AddSingleton<IRepository, BooRepository>()
                .AddSingleton<IRepository, FooRepository>()
                .AddSingleton<IRepository, OtherFooRepository>()
                .BuildProvider();

            var array = provider.GetService<IRepository[]>();

            Assert.Equal(3, array.Length);
        }

        [Fact]
        public void Resolve_Array_WithOneElement()
        {
            var provider = _builder
                .AddSingleton<IRepository, BooRepository>()
                .BuildProvider();

            var array = provider.GetService<IRepository[]>();

            Assert.Single(array);
            Assert.IsType<BooRepository>(array[0]);
        }
        
        [Fact]
        public void Resolve_MultiThreading()
        {
            var provider = _builder
                .AddInstance<ILogger>(new Logger())
                .AddGenericSingleton(typeof(IMapper<>), typeof(CompiledMapper<>))
                .AddSingleton<IFooService, FooService>()
                .AddSingleton<IFooRepository, FooRepository>()
                .AddSingleton<IBooService, BooService>()
                .AddSingleton<IBooRepository, BooRepository>()
                .AddSingleton<SomethingController>()
                .BuildProvider();

            var resolvedControllers = new ConcurrentBag<SomethingController>();
            for (var i = 0; i < 10; i++)
            {
                Task.Run(() =>
                {
                    var controller = provider.GetService<SomethingController>();
                    resolvedControllers.Add(controller);
                });
            }

            foreach (var controller in resolvedControllers)
            {
                foreach (var otherController in resolvedControllers)
                {
                    Assert.Same(controller, otherController);
                    Assert.Same(controller.Logger, otherController.Logger);
                    Assert.Same(controller.BooService, otherController.BooService);
                    Assert.Same(controller.FooService, otherController.FooService);
                    Assert.Same(controller.FooService.Configuration, otherController.FooService.Configuration);
                    Assert.Same(controller.FooService.Mapper, otherController.FooService.Mapper);
                    Assert.Same(controller.FooService.Repository, otherController.FooService.Repository);
                }
            }
        }

        [Fact]
        public void Throw_Resolve_NotRegistered()
        {
            var provider = _builder.BuildProvider();
            Assert.Throws<KeyNotFoundException>(() => provider.GetRequiredService(typeof(IManager<>)));
        }
        
        [Fact]
        public void Throw_Resolve_ArrayElementsNotRegistered()
        {
            var provider = _builder.BuildProvider();
            Assert.Throws<KeyNotFoundException>(() => provider.GetRequiredService<IRepository[]>());
        }
    }
}