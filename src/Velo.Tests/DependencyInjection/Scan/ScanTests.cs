using System;
using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.Settings.Provider;
using Velo.Settings.Sources;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;

namespace Velo.Tests.DependencyInjection.Scan
{
    public class ScanTests : TestClass
    {
        private readonly DependencyCollection _dependencies;
        
        public ScanTests()
        {
            _dependencies = new DependencyCollection()
                .AddSingleton<ISettingsProvider>(ctx => 
                    new SettingsProvider(
                        Array.Empty<ISettingsSource>(), 
                        ctx.GetRequired<IConvertersCollection>()))
                .AddJsonConverter()
                .AddSingleton<ISession, Session>();
        }

        [Fact]
        public void Scan_Scoped()
        {
            var provider = _dependencies
                .Scan(scanner => scanner
                    .AssemblyOf<IRepository>()
                    .ScopedOf<IRepository>())
                .BuildProvider();
            
            var repositories1 = provider.GetRequired<IRepository[]>();

            using (var scope = provider.StartScope())
            {
                var repositories2 = scope.GetRequired<IRepository[]>();
                for (var i = 0; i < repositories1.Length; i++)
                {
                    Assert.NotEqual(repositories1[i], repositories2[i]);
                }
            }
        }
        
        [Fact]
        public void Scan_Scope_Generic()
        {
            var provider = _dependencies
                .Scan(scanner => scanner
                    .AssemblyOf<BooRepository>()
                    .ScopedOf(typeof(IRepository<>)))
                .BuildProvider();
            
            var booRepository = provider.GetRequired<IRepository<Boo>>();
            Assert.NotNull(booRepository);
            Assert.Same(booRepository, provider.GetRequired<IRepository<Boo>>());

            using (var scope = provider.StartScope())
            {
                Assert.NotSame(booRepository, scope.GetRequired<IRepository<Boo>>());
            }
            
            var fooRepository = provider.GetRequired<IRepository<Foo>>();
            Assert.NotNull(fooRepository);
            Assert.Same(fooRepository, provider.GetRequired<IRepository<Foo>>());
            
            using (var scope = provider.StartScope())
            {
                Assert.NotSame(fooRepository, scope.GetRequired<IRepository<Foo>>());
            }
        }
        
        [Fact]
        public void Scan_Singleton()
        {
            var provider = _dependencies
                .Scan(scanner => scanner
                    .AssemblyOf<IRepository>()
                    .SingletonOf<IRepository>())
                .BuildProvider();
            
            var repositories1 = provider.GetRequired<IRepository[]>();
            var repositories2 = provider.GetRequired<IRepository[]>();

            for (var i = 0; i < repositories1.Length; i++)
            {
                Assert.Equal(repositories1[i], repositories2[i]);
            }
        }
        
        [Fact]
        public void Scan_Singleton_Generic()
        {
            var provider = _dependencies
                .Scan(scanner => scanner
                    .AssemblyOf<BooRepository>()
                    .SingletonOf(typeof(IRepository<>)))
                .BuildProvider();
            
            var booRepository = provider.GetRequired<IRepository<Boo>>();
            Assert.NotNull(booRepository);
            Assert.Same(booRepository, provider.GetRequired<IRepository<Boo>>());
            
            var fooRepository = provider.GetRequired<IRepository<Foo>>();
            Assert.NotNull(fooRepository);
            Assert.Same(fooRepository, provider.GetRequired<IRepository<Foo>>());
        }
        
        [Fact]
        public void Scan_Transient()
        {
            var provider = _dependencies
                .Scan(scanner => scanner
                    .AssemblyOf<IRepository>()
                    .TransientOf<IRepository>())
                .BuildProvider();
            
            var repositories1 = provider.GetRequired<IRepository[]>();
            var repositories2 = provider.GetRequired<IRepository[]>();

            for (var i = 0; i < repositories1.Length; i++)
            {
                Assert.NotEqual(repositories1[i], repositories2[i]);
            }
        }
        
        [Fact]
        public void Scan_Transient_Generic()
        {
            var provider = _dependencies
                .Scan(scanner => scanner
                    .AssemblyOf<BooRepository>()
                    .TransientOf(typeof(IRepository<>)))
                .BuildProvider();
            
            var booRepository = provider.GetRequired<IRepository<Boo>>();
            Assert.NotNull(booRepository);
            Assert.NotSame(booRepository, provider.GetRequired<IRepository<Boo>>());
            
            var fooRepository = provider.GetRequired<IRepository<Foo>>();
            Assert.NotNull(fooRepository);
            Assert.NotSame(fooRepository, provider.GetRequired<IRepository<Foo>>());
        }
    }
}