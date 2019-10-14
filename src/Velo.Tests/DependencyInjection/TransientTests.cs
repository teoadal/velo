using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Velo.Mapping;
using Velo.Serialization;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.DependencyInjection
{
    public class TransientTests : TestBase
    {
        private readonly DependencyCollection _builder;

        public TransientTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<JConverter>();
        }

        [Fact]
        public void Activator()
        {
            var provider = _builder
                .AddTransient<ISession, Session>()
                .BuildProvider();

            var first = provider.GetService<ISession>();
            var second = provider.GetService<ISession>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Activator_MultiThreading()
        {
            var provider = _builder
                .AddTransient<ISession, Session>()
                .AddTransient<IFooRepository, FooRepository>()
                .AddGenericTransient(typeof(IMapper<>), typeof(CompiledMapper<>))
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
            var provider = _builder
                .AddTransient<ISession>(ctx => new Session(ctx.GetService<JConverter>()))
                .BuildProvider();

            var first = provider.GetService<ISession>();
            var second = provider.GetService<ISession>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Builder_MultiThreading()
        {
            var provider = _builder
                .AddTransient<ISession, Session>()
                .AddTransient<IFooRepository, FooRepository>()
                .AddGenericTransient(typeof(IMapper<>), typeof(CompiledMapper<>))
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
            var provider = _builder
                .AddGenericTransient(typeof(List<>))
                .BuildProvider();

            var first = provider.GetService<List<int>>();
            var second = provider.GetService<List<int>>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Generic_With_Contract()
        {
            var provider = _builder
                .AddGenericTransient(typeof(IList<>), typeof(List<>))
                .BuildProvider();

            var first = provider.GetService<IList<int>>();
            var second = provider.GetService<IList<int>>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Throw_Not_Generic_Contract()
        {
            var builder = new DependencyCollection();

            Assert.Throws<InvalidOperationException>(() =>
                builder.AddGenericTransient(typeof(IFooRepository), typeof(FooRepository)));
        }

        [Fact]
        public void Throw_Not_Generic_Implementation()
        {
            var builder = new DependencyCollection();

            Assert.Throws<InvalidOperationException>(() =>
                builder.AddGenericTransient(typeof(IRepository<>), typeof(FooRepository)));
        }
    }
}