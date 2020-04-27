using System.IO;
using AutoFixture.Xunit2;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.DependencyInjection;
using Velo.Serialization;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization
{
    public class JConverterShould : TestClass
    {
        private readonly JConverter _converter;
        
        public JConverterShould(ITestOutputHelper output) : base(output)
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

        [Theory]
        [AutoData]
        public void SerializeToTextWriter(BigObject obj)
        {
            var writer = new StringWriter();
            _converter.Serialize(obj, writer);
            
            var serialized = writer.ToString();
            var result = JsonConvert.DeserializeObject<BigObject>(serialized);
            
            result.Should().BeEquivalentTo(obj);
        }
        
        [Fact]
        public void SerializeNullToTextWriter()
        {
            var writer = new StringWriter();
            _converter.Serialize(null, writer);
            
            var serialized = writer.ToString();
            var result = JsonConvert.DeserializeObject<BigObject>(serialized);
            
            result.Should().BeNull();
        }
    }
}