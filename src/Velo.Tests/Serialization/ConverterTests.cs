using AutoFixture.Xunit2;
using Velo.TestsModels;
using Velo.TestsModels.Boos;
using Velo.TestsModels.Foos;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Serialization
{
    public class ConverterTests: TestBase
    {
        private readonly JConverter _converter;
        
        public ConverterTests(ITestOutputHelper output) : base(output)
        {
            _converter = new JConverter();
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