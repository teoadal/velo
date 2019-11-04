using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.DependencyInjection
{
    public class ScanTests : TestBase
    {
        private readonly DependencyCollection _collection;
        
        public ScanTests(ITestOutputHelper output) : base(output)
        {
            _collection = new DependencyCollection()
                .AddSingleton<IConfiguration, Configuration>()
                .AddSingleton<JConverter>()
                .AddSingleton<ISession, Session>();
        }

        [Fact]
        public void Scan_Scoped()
        {
            var provider = _collection
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
        public void Scan_Singleton()
        {
            var provider = _collection
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
            var provider = _collection
                .Scan(scanner => scanner
                    .AssemblyOf<BooRepository>()
                    .SingletonOfGenericInterface(typeof(IRepository<>)))
                .BuildProvider();
            
            var booRepository = provider.GetRequiredService<IRepository<Boo>>();
            Assert.NotNull(booRepository);
            
            var fooRepository = provider.GetRequiredService<IRepository<Foo>>();
            Assert.NotNull(fooRepository);
        }
        
        [Fact]
        public void Scan_Transient()
        {
            var provider = _collection
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
    }
}