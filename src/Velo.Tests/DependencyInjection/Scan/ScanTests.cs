using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.Settings;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.DependencyInjection.Scan
{
    public class ScanTests : TestClass
    {
        private readonly DependencyCollection _dependencies;
        
        public ScanTests(ITestOutputHelper output) : base(output)
        {
            _dependencies = new DependencyCollection()
                .AddSingleton<IConfiguration>(ctx => new Configuration())
                .AddSingleton<JConverter>()
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
            
            var repositories1 = provider.GetRequiredService<IRepository[]>();

            using (var scope = provider.CreateScope())
            {
                var repositories2 = scope.GetRequiredService<IRepository[]>();
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
            
            var booRepository = provider.GetRequiredService<IRepository<Boo>>();
            Assert.NotNull(booRepository);
            Assert.Same(booRepository, provider.GetRequiredService<IRepository<Boo>>());

            using (var scope = provider.CreateScope())
            {
                Assert.NotSame(booRepository, scope.GetRequiredService<IRepository<Boo>>());
            }
            
            var fooRepository = provider.GetRequiredService<IRepository<Foo>>();
            Assert.NotNull(fooRepository);
            Assert.Same(fooRepository, provider.GetRequiredService<IRepository<Foo>>());
            
            using (var scope = provider.CreateScope())
            {
                Assert.NotSame(fooRepository, scope.GetRequiredService<IRepository<Foo>>());
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
            
            var repositories1 = provider.GetRequiredService<IRepository[]>();
            var repositories2 = provider.GetRequiredService<IRepository[]>();

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
            
            var booRepository = provider.GetRequiredService<IRepository<Boo>>();
            Assert.NotNull(booRepository);
            Assert.Same(booRepository, provider.GetRequiredService<IRepository<Boo>>());
            
            var fooRepository = provider.GetRequiredService<IRepository<Foo>>();
            Assert.NotNull(fooRepository);
            Assert.Same(fooRepository, provider.GetRequiredService<IRepository<Foo>>());
        }
        
        [Fact]
        public void Scan_Transient()
        {
            var provider = _dependencies
                .Scan(scanner => scanner
                    .AssemblyOf<IRepository>()
                    .TransientOf<IRepository>())
                .BuildProvider();
            
            var repositories1 = provider.GetRequiredService<IRepository[]>();
            var repositories2 = provider.GetRequiredService<IRepository[]>();

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
            
            var booRepository = provider.GetRequiredService<IRepository<Boo>>();
            Assert.NotNull(booRepository);
            Assert.NotSame(booRepository, provider.GetRequiredService<IRepository<Boo>>());
            
            var fooRepository = provider.GetRequiredService<IRepository<Foo>>();
            Assert.NotNull(fooRepository);
            Assert.NotSame(fooRepository, provider.GetRequiredService<IRepository<Foo>>());
        }
    }
}