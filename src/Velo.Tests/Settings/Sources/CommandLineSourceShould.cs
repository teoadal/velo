using System;
using System.Collections.Generic;
using FluentAssertions;
using Velo.Serialization.Models;
using Velo.Settings.Sources;
using Xunit;

namespace Velo.Tests.Settings.Sources
{
    public class CommandLineSourceShould : TestClass
    {
        [Fact]
        public void EvaluateNestedValues()
        {
            var source = new CommandLineSource(new[] {"boo.foo.value=true"});
            source.TryGet(out var jsonObject);

            var data = ((JsonObject) ((JsonObject) jsonObject["boo"])["foo"])["value"];
            ((JsonValue) data).Value.Should().Be("true");
        } 
        
        [Fact]
        public void NotReturnValuesIfEmptyArgs()
        {
            var source = new CommandLineSource(Array.Empty<string>());
            source.TryGet(out _).Should().BeFalse();
        }

        [Fact]
        public void NotReturnValuesIfNullArgs()
        {
            var source = new CommandLineSource(null);
            source.TryGet(out _).Should().BeFalse();
        }

        [Fact]
        public void ReturnValues()
        {
            var source = new CommandLineSource(new[] {"--property", "23"});
            source.TryGet(out var result).Should().BeTrue();
            result.Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(Args))]
        public void ReturnValidValues(string[] args, string propertyName, string expectedValue)
        {
            new CommandLineSource(args).TryGet(out var result);

            var property = (JsonValue) result[propertyName];
            property.Value.Should().Be(expectedValue);
        }

        public static IEnumerable<object[]> Args => new[]
        {
            new object[] {new[] {"--property", "23"}, "property", "23"},
            new object[] {new[] {"--property", "23.55"}, "property", "23.55"},
            new object[] {new[] {"--something", "testValue", $"--property", "\"test data\""}, "property", "test data"},
            new object[] {new[] {"property=23"}, "property", "23"},
            new object[] {new[] {"property=23.55"}, "property", "23.55"},
            new object[] {new[] {"something=testValue", $"property=\"test data\""}, "property", "test data"}
        };
    }
}