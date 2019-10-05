using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Dependencies
{
    public class ScanDependenciesTests : TestBase
    {
        private readonly DependencyBuilder _builder;
        
        public ScanDependenciesTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyBuilder()
                .AddSingleton<IConfiguration, Configuration>()
                .AddTransient<ISession, Session>()
                .AddSingleton<JConverter>();
        }

        [Fact]
        public void Scan_Assignable()
        {
            var container = _builder
                .Scan(scanner => scanner
                    .Assembly(typeof(IRepository).Assembly)
                    .RegisterAsSingleton<IRepository>())
                .BuildContainer();

            var repositories = container.Resolve<IRepository[]>();
            Assert.Equal(3, repositories.Length);
            Assert.Contains(repositories, r => r.GetType() == typeof(BooRepository));
            Assert.Contains(repositories, r => r.GetType() == typeof(FooRepository));
            Assert.Contains(repositories, r => r.GetType() == typeof(OtherFooRepository));
        }

        [Fact]
        public void Scan_Generic_Interface_Implementations()
        {
            var container = _builder
                .Scan(scanner => scanner
                    .Assembly(typeof(IRepository).Assembly)
                    .RegisterGenericInterfaceAsSingleton(typeof(IRepository<>)))
                .BuildContainer();

            var booRepository = container.Resolve<IRepository<Boo>>();
            Assert.NotNull(booRepository);

            var fooRepository = container.Resolve<IRepository<Foo>>();
            Assert.NotNull(fooRepository);
            
            var fooRepositories = container.Resolve<IRepository<Foo>[]>();
            Assert.Contains(fooRepositories, repo => repo.GetType() == typeof(FooRepository));
            Assert.Contains(fooRepositories, repo => repo.GetType() == typeof(OtherFooRepository));
        }
    }
}