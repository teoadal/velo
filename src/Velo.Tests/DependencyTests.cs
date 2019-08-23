using System.Collections.Generic;

using Velo.Dependencies;
using Velo.Mapping;
using Velo.Serialization;
using Velo.TestsModels;
using Velo.TestsModels.Services;

using Xunit;

namespace Velo
{
    public class DependencyTests
    {
        [Fact]
        public void Factory_Activator()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddFactory<ISession, Session>()
                .Build();

            var first = container.Resolve<ISession>();
            var second = container.Resolve<ISession>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Factory_Array()
        {
        }

        [Fact]
        public void Factory_Builder()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddFactory<ISession>(ctx => new Session(ctx.Resolve<JConverter>()))
                .Build();

            var first = container.Resolve<ISession>();
            var second = container.Resolve<ISession>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Factory_Generic()
        {
            var container = new DependencyBuilder()
                .AddGenericFactory(typeof(List<>))
                .Build();

            var first = container.Resolve<List<int>>();
            var second = container.Resolve<List<int>>();

            Assert.NotSame(first, second);
        }

        [Fact]
        public void Singleton_Activator()
        {
            var container = new DependencyBuilder()
                .AddSingleton<JConverter>()
                .AddSingleton<ISession, Session>()
                .Build();

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
                .Build();

            var first = container.Resolve<ISession>();
            var second = container.Resolve<ISession>();

            Assert.Same(first, second);
        }

        [Fact]
        public void Singleton_Generic()
        {
            var container = new DependencyBuilder()
                .AddGenericSingleton(typeof(CompiledMapper<>))
                .Build();

            var first = container.Resolve<CompiledMapper<Boo>>();
            var second = container.Resolve<CompiledMapper<Boo>>();

            Assert.Same(first, second);
        }

        [Fact]
        public void Singleton_Instance()
        {
            var container = new DependencyBuilder()
                .AddSingleton(new JConverter())
                .Build();

            var first = container.Resolve<JConverter>();
            var second = container.Resolve<JConverter>();

            Assert.Same(first, second);
        }
    }
}