using AutoFixture.Xunit2;
using Velo.DependencyInjection;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Serialization
{
    public class ConverterTests: TestClass
    {
        private readonly JConverter _converter;
        
        public ConverterTests(ITestOutputHelper output) : base(output)
        {
            _converter = new JConverter();
        }

        [Fact]
        public void Resolve()
        {
            var provider = new DependencyCollection()
                .AddJsonConverter()
                .BuildProvider();
            
            Assert.NotNull(provider.GetService<JConverter>());
            Assert.NotNull(provider.GetService<IConvertersCollection>());
        }
        
        [Theory, AutoData]
        public void PrepareConverter(BigObject bigObject)
        {
            _converter.PrepareConverterFor<BigObject>();
            _converter.PrepareConverterFor<Foo>();
            _converter.PrepareConverterFor<Boo>();
            
            Assert.NotEmpty(_converter.Serialize(bigObject));
        }
        
    }
}