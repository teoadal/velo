using System;
using System.Collections.Generic;
using FluentAssertions;
using Velo.Serialization.Models;
using Velo.Settings.Sources;
using Velo.TestsModels;
using Xunit;
using Xunit.Abstractions;

namespace Velo.Tests.Settings.Sources
{
    public class EnvironmentSourceShould : TestClass
    {
        private readonly EnvironmentSource _source;

        public EnvironmentSourceShould(ITestOutputHelper output) : base(output)
        {
            _source = new EnvironmentSource();
        }

        [Fact]
        public void ReturnValues()
        {
            _source.TryGet(out _).Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(Variables))]
        public void ReturnValidValues(string propertyName, string expectedValue)
        {
            Environment.SetEnvironmentVariable(propertyName, expectedValue);

            _source.TryGet(out var result);

            var value = (JsonValue) result[propertyName];
            value.Value.Should().Be(expectedValue);
        }

        public static IEnumerable<object[]> Variables => new[]
        {
            new object[] {"test", "testValue"},
            new object[] {"test", "23"}
        };
    }
}