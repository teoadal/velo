using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Velo.Mapping;
using Velo.Serialization;
using Velo.Settings;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.DependencyInjection
{
    public class TransientTests : TestClass
    {
        private readonly DependencyCollection _dependencies;

        public TransientTests(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection()
                .AddSingleton<IConfiguration>(_ => new Configuration())
                .AddSingleton<JConverter>();
        }

        [Fact]
        public void Compile()
        {
            var provider = _dependencies
                .AddTransient<ISession, Session>()
                .BuildProvider();

            var first = provider.GetService<ISession>();
            var second = provider.GetService<ISession>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Compile_MultiThreading()
        {
            var provider = _dependencies
                .AddTransient<ISession, Session>()
                .AddTransient<IFooRepository, FooRepository>()
                .AddTransient(typeof(IMapper<>), typeof(CompiledMapper<>))
                .AddTransient<FooService>()
                .BuildProvider();
            
            var firstTask = Task.Run(() => provider.GetService<FooService>());
            var secondTask = Task.Run(() => provider.GetService<FooService>());
            
            Task.WaitAll(firstTask, secondTask);

            var firstService = firstTask.Result;
            var secondService = secondTask.Result;
            Assert.NotSame(firstService, secondService);
            Assert.NotSame(firstService.Mapper, secondService.Mapper);
            Assert.NotSame(firstService.Repository, secondService.Repository);
            Assert.NotSame(firstService.Repository.Session, secondService.Repository.Session);
        }
        
        [Fact]
        public void Builder()
        {
            var provider = _dependencies
                .AddTransient<ISession>(ctx => new Session(ctx.GetService<JConverter>()))
                .BuildProvider();

            var first = provider.GetService<ISession>();
            var second = provider.GetService<ISession>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Builder_MultiThreading()
        {
            var provider = _dependencies
                .AddTransient<ISession, Session>()
                .AddTransient<IFooRepository, FooRepository>()
                .AddTransient(typeof(IMapper<>), typeof(CompiledMapper<>))
                .AddTransient(ctx => new FooService(
                    ctx.GetService<IConfiguration>(),
                    ctx.GetService<IMapper<Foo>>(),
                    ctx.GetService<IFooRepository>()))
                .BuildProvider();
            
            var firstTask = Task.Run(() => provider.GetService<FooService>());
            var secondTask = Task.Run(() => provider.GetService<FooService>());

            Task.WaitAll(firstTask, secondTask);

            var firstService = firstTask.Result;
            var secondService = secondTask.Result;
            Assert.NotSame(firstService, secondService);
            Assert.NotSame(firstService.Mapper, secondService.Mapper);
            Assert.NotSame(firstService.Repository, secondService.Repository);
            Assert.NotSame(firstService.Repository.Session, secondService.Repository.Session);
        }
        
        [Fact]
        public void Generic()
        {
            var provider = _dependencies
                .AddTransient(typeof(List<>))
                .BuildProvider();

            var first = provider.GetService<List<int>>();
            var second = provider.GetService<List<int>>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Generic_With_Contract()
        {
            var provider = _dependencies
                .AddTransient(typeof(IList<>), typeof(List<>))
                .BuildProvider();

            var first = provider.GetService<IList<int>>();
            var second = provider.GetService<IList<int>>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Two_Contracts()
        {
            var implementation = typeof(FooRepository);
            var contracts = new[] {implementation, typeof(IRepository<Foo>)};

            var provider = _dependencies
                .AddSingleton<ISession, Session>()
                .AddDependency(contracts, implementation, DependencyLifetime.Transient)
                .BuildProvider();

            var byImplementation = provider.GetRequiredService<FooRepository>();
            var byInterface = provider.GetRequiredService<IRepository<Foo>>();
            
            Assert.NotSame(byImplementation, byInterface);
            Assert.IsType<FooRepository>(byInterface);
        }

        [Fact]
        public void Throw_Not_Generic_Implementation()
        {
            var builder = new DependencyCollection();

            Assert.Throws<InvalidOperationException>(() =>
                builder.AddTransient(typeof(IRepository<>), typeof(FooRepository)));
        }
    }
}