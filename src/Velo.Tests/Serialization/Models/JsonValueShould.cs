using AutoFixture.Xunit2;
using FluentAssertions;
using Velo.Serialization.Models;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Serialization.Models
{
    public sealed class JsonValueShould : TestClass
    {
        public JsonValueShould(ITestOutputHelper output) : base(output)
        {
        }
        
        [Fact]
        public void BeEqual()
        {
            JsonValue.Boolean(false).Should().Be(JsonValue.False);
            JsonValue.Boolean(true).Should().Be(JsonValue.True);

            JsonValue.Number(0).Should().Be(JsonValue.Zero);
            JsonValue.Number(25f).Should().Be(JsonValue.Number(25));

            JsonValue.String(string.Empty).Should().Be(JsonValue.StringEmpty);
            JsonValue.String("abc").Should().Be(JsonValue.String("abc"));
        }
        
        [Theory, AutoData]
        public void HasValidHashCode(string str, int number)
        {
            JsonValue.String(str).GetHashCode().Should().Be(str.GetHashCode());
            JsonValue.Number(number).GetHashCode().Should().Be(number.ToString().GetHashCode());
        }
        
        [Fact]
        public void HasValidType()
        {
            JsonValue.Boolean(true).Type.Should().Be(JsonDataType.True);
            JsonValue.True.Type.Should().Be(JsonDataType.True);
            JsonValue.Boolean(false).Type.Should().Be(JsonDataType.False);
            JsonValue.False.Type.Should().Be(JsonDataType.False);

            JsonValue.Null.Type.Should().Be(JsonDataType.Null);

            JsonValue.Zero.Type.Should().Be(JsonDataType.Number);
            JsonValue.Number(25).Type.Should().Be(JsonDataType.Number);
            JsonValue.Number(25f).Type.Should().Be(JsonDataType.Number);

            JsonValue.String("abc").Type.Should().Be(JsonDataType.String);
            JsonValue.StringEmpty.Type.Should().Be(JsonDataType.String);
        }

    }
}