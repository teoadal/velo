using System.Collections.Generic;
using Velo.Serialization;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Dependencies
{
    public class TransientTests : TestBase
    {
        public TransientTests(ITestOutputHelper output) : base(output)
        {
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