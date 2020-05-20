using System;
using System.Reflection;
using FluentAssertions;
using Velo.DependencyInjection;
using Velo.Mapping;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;

namespace Velo.Tests.DependencyInjection
{
    public class DependencyProviderShould : TestClass
    {
        private readonly DependencyProvider _provider;

        public DependencyProviderShould()
        {
            _provider = new DependencyCollection()
                .AddScoped(ctx => new Boo())
                .AddSingleton(ctx => new Foo())
                .AddTransient(ctx => new object())
                .BuildProvider();
        }

        [Fact]
        public void CreateScope()
        {
            using var firstScope = _provider.StartScope();
            var firstBoo = firstScope.Get<Boo>();
            firstBoo.Should().Be(firstScope.GetService(typeof(Boo)));

            using var secondScope = _provider.StartScope();
            var secondBoo = secondScope.Get<Boo>();
            secondBoo.Should().Be(secondScope.GetService(typeof(Boo)));

            firstBoo.Should().NotBe(secondBoo);
        }

        [Fact]
        public void GetService()
        {
            using (var scope = _provider.StartScope())
            {
                var scoped = scope.Get(typeof(Boo));
                scoped.Should().NotBeNull();
            }

            var singleton = _provider.Get(typeof(Foo));
            singleton.Should().NotBeNull();

            var transient = _provider.Get(typeof(object));
            transient.Should().NotBeNull();
        }

        [Fact]
        public void GetNotRequiredService()
        {
            var transient = _provider.Get(typeof(PropertyInfo));
            transient.Should().BeNull();
        }

        [Fact]
        public void ThrowActivate()
        {
            Assert.Throws<InvalidOperationException>(() => _provider.Activate<IBooRepository>());
            Assert.Throws<InvalidOperationException>(() => _provider.Activate(typeof(CompiledMapper<>)));
            Assert.Throws<InvalidOperationException>(() => _provider.Activate(typeof(IMapper<>)));
        }

        [Fact]
        public void ThrowIfDisposed()
        {
            _provider.Dispose();

            Assert.Throws<ObjectDisposedException>(() => _provider.Get<IBooRepository>());
        }
    }
}