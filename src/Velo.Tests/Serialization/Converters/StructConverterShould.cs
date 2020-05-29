using System.Drawing;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using Velo.Serialization;
using Xunit;

namespace Velo.Tests.Serialization.Converters
{
    public class StructConverterShould : TestClass
    {
        private readonly JConverter _converter;

        public StructConverterShould()
        {
            _converter = new JConverter(BuildConvertersCollection());
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Deserialize<T>(T value)
        {
            var json = JsonConvert.SerializeObject(value);

            _converter
                .Deserialize<T>(json)
                .Should().BeEquivalentTo(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Read<T>(T value)
        {
            var converter = _converter.Converters.Get<T>();

            var jsonObject = converter.Write(value);

            converter
                .Read(jsonObject)
                .Should().BeEquivalentTo(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void ReadObject<T>(T value)
        {
            var converter = _converter.Converters.Get<T>();

            var jsonObject = converter.Write(value);

            converter
                .ReadObject(jsonObject)
                .Should().BeEquivalentTo(value);
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void Serialize<T>(T value)
        {
            var stringWriter = new StringWriter();
            _converter.Serialize(value, stringWriter);

            var actual = stringWriter.ToString();
            actual.Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void SerializeObject<T>(T value)
        {
            var converter = _converter.Converters.Get<T>();
            
            var stringWriter = new StringWriter();
            converter.SerializeObject(value, stringWriter);

            var actual = stringWriter.ToString();
            actual.Should().Be(JsonConvert.SerializeObject(value));
        }
        
        [Theory]
        [MemberData(nameof(Values))]
        public void Write<T>(T value)
        {
            var converter = _converter.Converters.Get<T>();
            
            var jsonValue = converter.Write(value);

            jsonValue
                .Serialize()
                .Should().Be(JsonConvert.SerializeObject(value));
        }

        [Theory]
        [MemberData(nameof(Values))]
        public void WriteObject<T>(T value)
        {
            var converter = _converter.Converters.Get<T>();
            
            var jsonValue = converter.WriteObject(value);

            jsonValue
                .Serialize()
                .Should().Be(JsonConvert.SerializeObject(value));
        }

        
        public static TheoryData<object> Values => new TheoryData<object>()
        {
            Size.Empty,
            new Size(),
            new Size(5, 10),
            Point.Empty,
            new Point(),
            new Point(5, 10)
        };
    }
}