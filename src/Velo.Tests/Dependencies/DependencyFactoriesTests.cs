using Velo.Serialization;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Domain;
using Velo.TestsModels.Foos;
using Velo.TestsModels.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Dependencies
{
    public class DependencyFactoriesTests : TestBase
    {
        private DependencyBuilder _builder;
        
        public DependencyFactoriesTests(ITestOutputHelper output) : base(output)
        {
            _builder = new DependencyBuilder()
                .AddTransient<ISession, Session>()
                .AddSingleton<JConverter>()
                .AddSingleton<IConfiguration, Configuration>();
        }

        [Fact]
        public void Array()
        {
            var container = _builder
                .AddSingleton<IRepository, FooRepository>()
                .AddSingleton<IRepository, BooRepository>()
                .BuildContainer();

            var first = container.Resolve<IRepository[]>();
            var second = container.Resolve<IRepository[]>();

            Assert.NotSame(first, second);

            for (var i = 0; i < first.Length; i++)
            {
                var firstRepository = first[i];
                var secondRepository = second[i];

                Assert.Same(firstRepository, secondRepository);
                Assert.Same(firstRepository.Configuration, secondRepository.Configuration);
                Assert.Same(firstRepository.Session, secondRepository.Session);
            }
        }
        
        [Fact]
        public void Array_OneElement()
        {
            var container = _builder.AddSingleton<IRepository, BooRepository>().BuildContainer();

            var array = container.Resolve<IRepository[]>();
            Assert.Single(array);
        }
        
        [Fact]
        public void Array_NoElements()
        {
            var container = _builder.BuildContainer();

            var array = container.Resolve<IRepository[]>();
            Assert.Empty(array);
        }
    }
}