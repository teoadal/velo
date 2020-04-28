using System;
using System.Reflection;
using FluentAssertions;
using Velo.DependencyInjection;
using Velo.Mapping;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection
{
    public class DependencyProviderShould : TestClass
    {
        private readonly DependencyProvider _provider;

        public DependencyProviderShould(ITestOutputHelper output) : base(output)
        {
            _provider = new DependencyCollection()
                .AddScoped(ctx => new Boo())
                .AddSingleton(ctx => new Foo())
                .AddTransient(ctx => new object())
                .BuildProvider();
        }

        [Fact]
        public void ActivateInstance()
        {
            
        }
        
        [Fact]
        public void GetService()
        {
            using (var scope = _provider.CreateScope())
            {
                var scoped = scope.GetService(typeof(Boo));
                scoped.Should().NotBeNull();
            }

            var singleton = _provider.GetService(typeof(Foo));
            singleton.Should().NotBeNull();

            var transient = _provider.GetService(typeof(object));
            transient.Should().NotBeNull();
        }

        [Fact]
        public void GetNotRequiredService()
        {
            var transient = _provider.GetService(typeof(PropertyInfo));
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

            Assert.Throws<ObjectDisposedException>(() => _provider.Activate(typeof(Boo)));
            Assert.Throws<ObjectDisposedException>(() => _provider.GetService<Boo>());
        }
    }
}